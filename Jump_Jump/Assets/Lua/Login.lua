Login = {}
local this = Login
require('Music')
local tex
local ui
local manager
function this.Awake(object)
    manager = object
    tex = Resources.Load('Textures/2B')
    manager : AddComponent(typeof(AudioSource))
    coroutine.start(Music.PlaySound)--协程开始
    ui = GameObject.Find('Panel')
    local image = ui.transform : Find('Image').gameObject : GetComponent('Image')
    local loginBtn = ui.transform : Find('Login').gameObject
    Util.AddSprite(image, 'Textures', '2B')
    UIEvent.AddButtonOnClick(loginBtn, LoginOnClick)
end

function LoginOnClick()
    SceneManagement.SceneManager.LoadScene("Jump")

end

