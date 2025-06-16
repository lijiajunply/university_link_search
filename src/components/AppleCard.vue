<template>
  <div
      :id="id"
      :style="style"
      class="bg-white dark:bg-black/50 shadow-lg hover:shadow-xl"
      :class="['apple-card', customClass]"
  >
    <div
        v-if="$slots.title"
        class="bg-white/60 dark:bg-black/30"
        :class="['apple-card-header', headerClass]"
        :style="headerStyle"
    >
      <slot name="title"/>
    </div>

    <div
        :class="['apple-card-context', contextClass]"
        :style="contextStyle"
    >
      <slot name="context"/>
    </div>
  </div>
</template>

<script setup>
defineProps({
  id: String,
  style: String,
  customClass: {
    type: String,
    default: ''
  },
  contextStyle: String,
  headerStyle: String,
  headerClass: String,
  contextClass: String
})
</script>

<style scoped>
.apple-card {
  border-radius: 20px;
  min-height: 300px;
  max-height: 100%;
  transition: .2s;

}

@media screen and (min-width: 768px) {
  .apple-card:hover {
    transform: scale(1.02);
    transition: box-shadow 0.3s, border-color 0.3s, transform 0.2s;
  }
}

@media screen and (max-width: 768px) {
  .apple-card:active {
    transform: scale(0.98);
  }
}

.apple-card .apple-card-header {
  border-radius: 20px 20px 0 0;
  padding: 15px 30px 1px;
}

.apple-card .apple-card-context {
  border-radius: 0 0 20px 20px;
  padding: 1px 30px 1px;
  overflow: auto;
}
</style>