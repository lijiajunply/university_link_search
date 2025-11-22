<script setup lang="ts">
import {useRoute, useRouter} from 'vue-router'
import {useAuthorizationStore} from "../stores/Authorization";
import {onMounted} from "vue";
import {type ClubData} from '../models/ClubData'

const route = useRoute()
const router = useRouter()
const authorizationStore = useAuthorizationStore()

onMounted(() => {
  const data = {
    token: route.query.token as string || '',
    id: route.query.sub as string || '',
    name: route.query.name as string || '',
    role: route.query.role as string || '',
  } as ClubData

  authorizationStore.setClubData(data)

  router.push('/')
})
</script>

<template>

</template>

<style scoped>

</style>