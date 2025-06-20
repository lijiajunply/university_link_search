// loginService.js

// 计算 SHA-1 的辅助函数
async function calculateSHA1(input) {
    const encoder = new TextEncoder()
    const data = encoder.encode(input)
    const hashBuf = await crypto.subtle.digest('SHA-1', data)
    const hashArr = Array.from(new Uint8Array(hashBuf))
    return hashArr.map(b => b.toString(16).padStart(2, '0')).join('')
}

// 对登录参数进行加密
async function encodeLoginParams({salt, username, password}) {
    if (!salt || !username || !password) {
        throw new Error('Invalid login parameters')
    }
    const encPassword = await calculateSHA1(`${salt}-${password}`)
    return {
        username,
        password: encPassword,
        captcha: ''
    }
}

// 从 set-cookie 字符串里提取 SESSION 和 __pstsid__
function parseCookie(setCookieHeader = '') {
    const parts = setCookieHeader.split(',')
    const cookies = []
    for (let part of parts) {
        if (part.includes('__pstsid__')) {
            cookies.push(part.split(';')[0])
        } else if (part.includes('SESSION=')) {
            const match = part.match(/SESSION=([^;]+)/)
            if (match) cookies.push(`SESSION=${match[1]}`)
        }
    }
    return cookies.join('; ')
}

// 发起 GET，返回文本 body 和原始 set-cookie 头
async function fetchWithCookies(url) {
    const resp = await fetch(url, {
        method: 'GET',
        credentials: 'include'  // 如果需要浏览器自动处理 cookie
    })
    if (!resp.ok) {
        throw new Error(`Failed to fetch ${url}: ${resp.status}`)
    }
    const body = await resp.text()
    // 注意：浏览器 fetch 无法直接读到 set-cookie，若在 Node.js 环境请用 node-fetch + tough-cookie
    const setCookie = resp.headers.get('set-cookie') || ''
    return {body, setCookie, finalUrl: resp.url}
}

// 获取 studentId 或 HTML 表单域
async function getCode(cookies) {
    try {
        const resp = await fetch('https://swjw.xauat.edu.cn/student/for-std/precaution', {
            method: 'GET',
            headers: {
                'Cookie': cookies
            },
            redirect: 'follow'
        })
        if (!resp.ok) return ''
        const finalUrl = new URL(resp.url).pathname
        const indexPrefix = '/student/precaution/index/'
        if (finalUrl.startsWith(indexPrefix)) {
            // URL 里带有 studentId
            return finalUrl.replace(indexPrefix, '')
        }
        // 否则解析 HTML body 中的 <input value="...">
        const html = await resp.text()
        const regex = /value="(.*?)">/g
        const matches = []
        let m
        while ((m = regex.exec(html)) !== null) {
            matches.push(m[1])
        }
        return matches.join(',')
    } catch (e) {
        console.warn(e)
        return ''
    }
}

// 主登录函数
export async function loginAsync(username, password) {
    // 1) 拿 salt + set-cookie
    const { body: salt, setCookie } = await fetchWithCookies(
        'https://swjw.xauat.edu.cn/student/login-salt'
    )
    if (!salt) {
        throw new Error('Failed to get salt')
    }
    const cookies = parseCookie(setCookie)
    localStorage.setItem('cookies', cookies)

    // 2) 构建加密参数
    const encoded = await encodeLoginParams({salt, username, password})

    // 3) 发起登录请求
    const loginResp = await fetch('https://swjw.xauat.edu.cn/student/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Cookie': cookies
        },
        body: JSON.stringify(encoded),
        credentials: 'include'
    })
    if (!loginResp.ok) {
        throw new Error('Login failed')
    }

    // 4) 再次访问获取 studentId 或表单值
    const studentId = await getCode(cookies)
    if (!studentId) {
        return {success: false}
    }
    localStorage.setItem('studentId', studentId)
    return {
        success: true,
        studentId,
        cookie: cookies
    }
}