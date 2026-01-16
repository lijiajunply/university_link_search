export const getIcon = (url: string): string => {
  // 移除协议部分
  const cleanUrl = url.replace(/^https?:\/\//, '');
  // 获取第一个斜杠之前的部分（即根域名）
  const rootDomain = cleanUrl.split('/')[0];
  // 拼接形成 favicon URL
  return `https://${rootDomain}/favicon.ico`;
}

export const checkIsWeiXin = (): boolean => {
  const ua = navigator.userAgent
  return !!/MicroMessenger/i.test(ua)
}
