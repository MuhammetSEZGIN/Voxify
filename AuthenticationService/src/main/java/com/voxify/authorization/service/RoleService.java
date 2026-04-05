package com.voxify.authorization.service;

import com.voxify.authorization.constant.RoleEnum;
import com.voxify.authorization.dtos.DeleteRoleRequest;
import com.voxify.authorization.dtos.RoleDto;
import com.voxify.authorization.dtos.UpdateRoleRequest;
import com.voxify.authorization.entity.Role;
import com.voxify.authorization.exceptions.RoleException;
import com.voxify.authorization.repository.RoleRepository;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.data.redis.core.StringRedisTemplate;
import org.springframework.stereotype.Service;

import java.util.Optional;
import java.util.Set;

@Service
@RequiredArgsConstructor
public class RoleService {
    private final RoleRepository roleRepository;
    private final StringRedisTemplate redisTemplate;

    @Cacheable(value = "clanRoles", key = "'u:' + #userId + '-c:' + #clanId")
    public RoleDto getRoles(String userId, String clanId) {

        Optional<Role> existingRole = roleRepository.findByUserIdAndClanId(
                userId, clanId);
        if (existingRole.isEmpty()) {
            throw new RoleException("Kullanıcı için rol bulunamadı: " + userId);
        }

        return mapToDto(existingRole.get());
    }

    @Transactional
    @CacheEvict(value = "clanRoles", key = "'u:' + #roleDto.userId + '-c:' + #roleDto.clanId")
    public RoleDto updateRole(UpdateRoleRequest roleDto) {
        if (RoleEnum.isValid(roleDto.getRoles())) {
            throw new RoleException("Geçersiz rol: " + roleDto.getRoles() +
                    ". İzin verilen roller: MEMBER, MODERATOR, ADMIN");
        }
        Role existingRole = roleRepository.findByUserIdAndClanId(
                        roleDto.getUserId(), roleDto.getClanId())
                .orElseThrow(() -> new RoleException("Kullanıcı: " + roleDto.getUserId() +
                        "ve " + roleDto.getClanId() + " için güncellenecek rol bulunamadı: "));

        existingRole.setRoles(roleDto.getRoles());
        Role updatedRole = roleRepository.save(existingRole);
        return mapToDto(updatedRole);
    }

    @Transactional
    @CacheEvict(value = "clanRoles", key = "'u:' + #userId + '-c:' + #clanId")
    public RoleDto createRole(String userId, String clanId, String role) {
        // Rol validasyonu
        if (!RoleEnum.isValid(role)) {
            throw new RoleException("Geçersiz rol: " + role +
                    ". İzin verilen roller: MEMBER, MODERATOR, ADMIN");
        }

        // Kullanıcının bu klandan zaten rolü güncellesin
        Optional<Role> existingRole = roleRepository.findByUserIdAndClanId(userId, clanId);
        if (existingRole.isPresent()) {
            Role roleToUpdate = existingRole.get();
            roleToUpdate.setRoles(role);
            Role updatedRole = roleRepository.save(roleToUpdate);
            return mapToDto(updatedRole);
        }
        Role newRole = Role.builder()
                .userId(userId)
                .clanId(clanId)
                .roles(role)
                .build();

        Role savedRole = roleRepository.save(newRole);
        return mapToDto(savedRole);
    }

    @Transactional
    public Optional<Role> deleteAllRolesByClanId(String clanId) {

        Optional<Role> result = roleRepository.deleteRolesByClanId(clanId);
        if (result.isPresent()) {
            // İlgili cache anahtarlarını temizle
            String pattern = "clanRoles::u:*-c:" + clanId;
            Set<String> keysToDelete = redisTemplate.keys(pattern);
            if (keysToDelete != null && !keysToDelete.isEmpty()) {
                redisTemplate.delete(keysToDelete);
            }
        }
        return result;
    }

    @Transactional
    @CacheEvict(value = "clanRoles", key = "'u:' + #deleteRoleRequest.userId + '-c:' + #deleteRoleRequest.clanId")
    public RoleDto deleteRole(DeleteRoleRequest deleteRoleRequest) {
        Role role = roleRepository.deleteRoleByClanIdAndUserId(
                deleteRoleRequest.getClanId(), deleteRoleRequest.getUserId());
        if (role != null) {
            roleRepository.delete(role);
            return mapToDto(role);
        }
        throw new RoleException("Silinecek rol bulunamadı: " + deleteRoleRequest.getUserId());
    }

    private RoleDto mapToDto(Role role) {
        return RoleDto.builder()
                .roleId(role.getRoleId())
                .userId(role.getUserId())
                .clanId(role.getClanId())
                .roles(role.getRoles())
                .build();
    }
}

