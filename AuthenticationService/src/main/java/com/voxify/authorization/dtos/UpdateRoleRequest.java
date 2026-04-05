package com.voxify.authorization.dtos;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
public class UpdateRoleRequest {

    @NotBlank(message = "userId boş olamaz")
    private String userId;
    @NotBlank(message = "clanId boş olamaz")
    private String clanId;
    @NotBlank(message = "rol adı boş olamaz")
    private String roles;
}
