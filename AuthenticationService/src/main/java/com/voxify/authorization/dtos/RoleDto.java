package com.voxify.authorization.dtos;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;

@Data
@Builder
@AllArgsConstructor
public class RoleDto {
    @NotBlank(message = "roleId boş olamaz")
    private String roleId;
    @NotBlank(message = "userId boş olamaz")
    private String userId;
    @NotBlank(message = "clanId boş olamaz")
    private String clanId;
    @NotBlank(message = "rol adı boş olamaz")
    private String roles;
}
