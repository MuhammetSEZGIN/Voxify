package com.voxify.authorization.constant;

import lombok.Getter;

@Getter
public enum EventType {
    ASSIGN_ROLE("ASSIGN_ROLE"),
    REMOVE_ROLE("REMOVE_ROLE"),
    REMOVE_ALL_ROLES("REMOVE_ALL_ROLES");

    private final String value;

    EventType(String value) {
        this.value = value;
    }
    public static boolean isValid(String role){
        try {
            EventType.valueOf(role);
            return true;
        } catch (IllegalArgumentException e) {
            return false;
        }
    }
}
