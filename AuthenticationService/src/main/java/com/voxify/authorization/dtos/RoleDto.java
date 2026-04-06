package com.voxify.authorization.dtos;

import jakarta.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;

import java.io.Serializable;

@Data
@Builder
@AllArgsConstructor
public class RoleDto implements Serializable {
    @NotBlank(message = "roleId boş olamaz")
    private String roleId;
    @NotBlank(message = "userId boş olamaz")
    private String userId;
    @NotBlank(message = "clanId boş olamaz")
    private String clanId;
    @NotBlank(message = "rol adı boş olamaz")
    private String roles;
}
