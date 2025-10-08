// loginService.js

// 主登录函数
export async function eduLogin(username, password) {
    const loginResp = await fetch('https://swjw.xauat.edu.cn/student/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username,
            password
        }),
        credentials: 'include'
    })

    if (!loginResp.ok) {
        return {
            success: false,
            message: `登录失败：${loginResp.status}`
        }
    }

    const {studentId, cookies} = await loginResp.json()

    localStorage.setItem('studentId', studentId)
    return {
        success: true,
        studentId,
        cookie: cookies
    }
}