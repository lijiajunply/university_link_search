import pako from 'pako';

/** 压缩文本字符串（浏览器兼容） */
export function zipText(str: string) {
    // 1. URI编码 → 2. Gzip压缩 → 3. 转为Base64
    const gzipped = pako.gzip(encodeURIComponent(str));
    return btoa(String.fromCharCode(...gzipped));
}

/** 解压文本字符串（浏览器兼容） */
export function unzipText(str: string) {
    // 1. Base64解码 → 2. Gzip解压 → 3. URI解码
    const binaryString = atob(str);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return decodeURIComponent(pako.ungzip(bytes, {to: 'string'}));
}

/** 压缩对象为JSON字符串 */
export function zipObj(obj: object) {
    return zipText(JSON.stringify(obj));
}

/** 解压并解析为对象 */
export function unzipObj(str: string) {
    return JSON.parse(unzipText(str));
}