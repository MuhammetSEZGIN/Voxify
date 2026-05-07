package com.voxify.authorization.dtos;

import jakarta.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class DeleteRoleRequest {
    @NotBlank(message = "userId boş olamaz")
    private String userId;
    @NotBlank(message = "clanId boş olamaz")
    private String clanId;

}
