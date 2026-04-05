package com.voxify.authorization.repository;

import com.voxify.authorization.entity.Role;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface RoleRepository extends JpaRepository<Role,String> {
    Optional<Role> findByUserIdAndClanId(String userId, String clanId);
    Role deleteRoleByClanIdAndUserId(String clanId, String userId);
    Optional<Role> deleteRolesByClanId(String clanId);
}
