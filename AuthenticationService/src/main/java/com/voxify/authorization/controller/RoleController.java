package com.voxify.authorization.controller;

import com.voxify.authorization.dtos.RoleDto;
import com.voxify.authorization.service.RoleService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

@RestController
@RequiredArgsConstructor
@RequestMapping("roles")
@Validated
public class RoleController {
    private final RoleService roleService;

    @GetMapping
    public ResponseEntity<RoleDto> getRoles(
            @Valid @RequestParam  String userId, String clanId) {
        return ResponseEntity.ok(roleService.getRoles(userId, clanId));
    }

//    @PutMapping
//    public ResponseEntity<RoleDto> updateRole(@Valid @RequestBody UpdateRoleRequest roleDto) {
//        return ResponseEntity.ok(roleService.updateRole(roleDto));
//    }
//
//    @DeleteMapping
//    public ResponseEntity<RoleDto> deleteRole(
//            @Valid @RequestBody DeleteRoleRequest deleteRoleRequest) {
//        return ResponseEntity.ok(roleService.deleteRole(deleteRoleRequest));
//    }
}