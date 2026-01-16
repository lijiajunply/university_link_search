import { ref, onMounted, onUnmounted } from 'vue';

export function useContextMenu(searchRef: any) {
  const isRightMenuVisible = ref(false);

  // 显示菜单
  const showMenu = (event: MouseEvent) => {
    event.preventDefault()
    isRightMenuVisible.value = true
  }

  // 隐藏菜单
  const hideMenu = (event: MouseEvent | Event) => {
    const target = event.target as HTMLElement;
    if (target.id === 'setting') {
      event.preventDefault()
      console.log('setting')
      return;
    }
    if (searchRef.value?.hide()) return;
    isRightMenuVisible.value = false
  }

  const handleKeydown = (event: KeyboardEvent) => {
    if (event.key === 'Escape') {
      hideMenu(event)
    }
  }

  const handleContextMenu = (event: MouseEvent) => {
    // 检查是否在输入框等元素上右键
    const target = event.target as HTMLElement;
    if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.isContentEditable || target.id === 'chooseEngine') {
      return // 允许默认右键菜单
    }

    showMenu(event)
  }

  onMounted(() => {
    document.addEventListener('keydown', handleKeydown)
    document.addEventListener('contextmenu', handleContextMenu)
  })

  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
    document.removeEventListener('contextmenu', handleContextMenu)
  })

  return {
    isRightMenuVisible,
    hideMenu
  }
}
