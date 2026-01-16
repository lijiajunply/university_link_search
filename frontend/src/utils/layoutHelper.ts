export const getCategoryColClass = (index: number, total: number): string => {
  const isLastOdd = index === total - 1 && total % 2 !== 0

  if (isLastOdd) {
    return 'col-span-8'
  }

  const i = index % 4 === 1 || index % 4 === 2 ? 5 : 3

  return i === 5 ? 'col-span-8 md:col-span-4 lg:col-span-5' : "col-span-8 md:col-span-4 lg:col-span-3";
}

export const getLinkColClass = (categoryIndex: number, total: number): string => {
  const isLastOdd = categoryIndex === total - 1 && total % 2 !== 0

  let lgColClass

  if (isLastOdd) {
    lgColClass = 'md:col-span-2 lg:col-span-1'
  } else {
    const i = categoryIndex % 4 === 1 || categoryIndex % 4 === 2 ? 15 : 9
    const col = i === 15 ? 2 : 3

    lgColClass = col === 2
        ? 'md:col-span-4 lg:col-span-2'
        : 'md:col-span-4 lg:col-span-3'
  }

  return `col-span-4 ${lgColClass} `
}
