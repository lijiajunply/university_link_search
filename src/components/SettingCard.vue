<template>
  <Teleport to="body">
    <div
        v-if="show"
        class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
        @click="closeModal"
    >
      <div
          class="bg-white rounded-lg p-6 w-11/12 max-w-md mx-auto shadow-xl"
          @click.stop
      >

      </div>
    </div>
  </Teleport>
</template>

<script setup>
defineProps({
  show: Boolean,
});

const emit = defineEmits(['update:show']);

function closeModal() {
  emit('update:show', false);
}
</script>

<style scoped>

</style>