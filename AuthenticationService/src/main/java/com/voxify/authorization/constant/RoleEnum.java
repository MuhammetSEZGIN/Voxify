package com.voxify.authorization.constant;

import lombok.Data;
import lombok.Getter;

@Getter
public enum RoleEnum {
    MEMBER("MEMBER"),
    ADMIN("ADMIN"),
    OWNER("OWNER");

    private final String value;

    RoleEnum(String value) {
        this.value = value;
    }

    public static boolean isValid(String role) {
        try {
            RoleEnum.valueOf(role);
            return true;
        } catch (IllegalArgumentException e) {
            return false;
        }
    }
}
