package com.voxify.authorization.dtos;

import com.fasterxml.jackson.annotation.JsonAlias;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ClanRoleEventDto {
    @JsonAlias({"UserId", "userId"})
    private  String userId;
    @JsonAlias({"ClanId", "clanId"})
    private  String clanId;
    @JsonAlias({"Role", "role"})
    private  String role; // Örn: "Member", "Admin", "Owner"
    @JsonAlias({"EventType", "eventType"})
    private  String eventType; // Örn: "ASSIGN_ROLE", "REMOVE_ROLE", "REMOVE_ALL_ROLES"

}
