package com.voxify.authorization.messaging;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.voxify.authorization.constant.EventType;
import com.voxify.authorization.dtos.ClanRoleEventDto;
import com.voxify.authorization.dtos.DeleteRoleRequest;
import com.voxify.authorization.dtos.RoleDto;
import com.voxify.authorization.entity.Role;
import com.voxify.authorization.service.RoleService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.core.ExchangeTypes;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.rabbit.annotation.Exchange;
import org.springframework.amqp.rabbit.annotation.Queue;
import org.springframework.amqp.rabbit.annotation.QueueBinding;
import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.stereotype.Component;

import java.nio.charset.StandardCharsets;
import java.util.Optional;

@Component
@Slf4j
@RequiredArgsConstructor
public class ClanServiceListener {

    private final RoleService roleService;
    private final ObjectMapper objectMapper; // Jackson JSON dönüştürücümüz

    @RabbitListener(bindings = @QueueBinding(
            value = @Queue(value = "auth-service-clan-group", durable = "true"),
            exchange = @Exchange(value = "Shared.Contracts:ClanRoleEventDto", type = ExchangeTypes.FANOUT)
    ))
    public void receiveMessage(Message amqpMessage) {
        try {
            // 1. Gelen mesajı filtreleri aşarak saf String'e çeviriyoruz
            String rawJson = new String(amqpMessage.getBody(), StandardCharsets.UTF_8);
            log.info("Saf JSON yakalandı: {}", rawJson);
            JsonNode rootNode = objectMapper.readTree(rawJson);
            JsonNode messageNode = rootNode.get("message");
            if (messageNode == null) {
                log.warn("Rabbitmq zarfında 'message' alanı bulunamadı!");
                return;
            }
            ClanRoleEventDto event = objectMapper.treeToValue(messageNode, ClanRoleEventDto.class);

            if (event == null || event.getEventType() == null) {
                log.warn("Event içeriği boş!");
                return;
            }

            // 3. İş Süreçleri
            EventType eventType = EventType.valueOf(event.getEventType());
            switch (eventType) {
                case ASSIGN_ROLE:
                    RoleDto result1= roleService.createRole(event.getUserId(), event.getClanId(), event.getRole());
                    log.info("Rol başarıyla atandı. {}", result1.toString());
                    break;
                case REMOVE_ROLE:
                    RoleDto result2= roleService.deleteRole(new DeleteRoleRequest(event.getUserId(), event.getClanId()));
                    log.info("Rol başarıyla kaldırıldı.{}", result2.toString());
                    break;
                case REMOVE_ALL_ROLES:
                    Optional<Role> result3 = roleService.deleteAllRolesByClanId(event.getClanId());
                    log.info("Tüm klan rolleri temizlendi:{}", result3.toString());
                    break;
                default:
                    log.warn("Bilinmeyen Event: {}", eventType);
            }

        } catch (Exception e) {
            // Hata olursa Catch bloğuna düşer, uygulaman çökmez.
            // İleride buraya kendi Retry veya DLQ mantığını yazabilirsin.
            log.error("Mesaj işlenirken hata oluştu: {}", e.getMessage(), e);
        }
    }
}