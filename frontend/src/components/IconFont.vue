<template>
  <div :class="style" :style="size ? `font-size:${size}px` : undefined">
    <svg class="icon" aria-hidden="true">
      <use :href="type"></use>
    </svg>
  </div>
</template>

<script setup>
defineProps({
  type: {
    type: String,
    required: true
  },
  style: String,
  size: Number
})
</script>