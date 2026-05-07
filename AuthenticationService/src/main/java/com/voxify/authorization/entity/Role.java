package com.voxify.authorization.entity;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Entity
@Table(name = "roles")
@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class Role {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private String roleId;
    @Column(nullable = false)
    private String userId;
    @Column(nullable = false)
    private String clanId;
    @Column(nullable = false)
    private String roles;
}
