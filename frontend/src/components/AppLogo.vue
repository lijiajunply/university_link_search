<template>
    <template v-if="icon">
        <img v-if="icon.startsWith('http')" v-lazy="icon" :alt="name" class="h-10 w-10 rounded" />
        <IconFont v-else :type="`#icon-${icon}`" class="text-[40px]" />
    </template>
    <img v-else v-lazy="getIcon(url)" :alt="name" class="h-10 w-10 rounded" />
</template>

<script setup lang="ts">
import IconFont from './IconFont.vue';
import { getIcon } from '../utils/urlHelper';

const props = defineProps<{
    url: string;
    name: string;
    icon: string;
}>();
</script>