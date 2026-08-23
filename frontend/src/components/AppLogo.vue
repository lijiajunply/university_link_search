<template>
    <template v-if="icon">
        <img
            v-if="icon.startsWith('http')"
            v-lazy="icon"
            :alt="name"
            class="rounded"
            :style="{ width: `${size}px`, height: `${size}px` }"
        />
        <IconFont v-else :type="`#icon-${icon}`" :size="size" />
    </template>
    <img
        v-else
        v-lazy="getIcon(url)"
        :alt="name"
        class="rounded"
        :style="{ width: `${size}px`, height: `${size}px` }"
    />
</template>

<script setup lang="ts">
import IconFont from './IconFont.vue';
import { getIcon } from '../utils/urlHelper';

withDefaults(defineProps<{
    url?: string;
    name?: string;
    icon?: string;
    size?: number;
}>(), {
    url: '',
    name: '',
    icon: '',
    size: 40,
});
</script>