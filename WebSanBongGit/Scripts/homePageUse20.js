const $  = document.querySelector.bind(document)
const $$ = document.querySelectorAll.bind(document)
function onChange() {
    console.log('onChange')
    const password = $('#password');
    const confirm = $('#again-password');
    if (confirm.value === password.value) {
        confirm.setCustomValidity('');
    } else {
        confirm.setCustomValidity('Passwords do not match');
    }
}
// hien password cho o password
function showAndHidePasword(){
    const showAndHidePasword = $('.showAndHide.pasword')
    const inPutPassword = $('#password')
    const showAndHidePasword1 = $('.showAndHide.pasword.login')
    const inPutPasswordlogin = $('#PASSWORD')
    let Iskt =  false
    let Iskt2 = false
    if (showAndHidePasword) {
        showAndHidePasword.onclick = (e) => {
            Iskt = !Iskt
            showAndHidePasword.classList.toggle('active', Iskt)

            Iskt ? inPutPassword.type = "text"
                : inPutPassword.type = "password"

        }
    }
   
    if (showAndHidePasword1) {
        showAndHidePasword1.onclick = (e) => {
            Iskt2 = !Iskt2
            showAndHidePasword1.classList.toggle('active', Iskt2)

            Iskt2 ? inPutPasswordlogin.type = "text"
                : inPutPasswordlogin.type = "password"

        }
    }
   
}
showAndHidePasword()

//hien password cho o againPassword
function showAndHideAgainpasword(){
    const showAndHideAgainpasword = $('.showAndHide.Againpasword')
    const inPutPassword_Again = $('#again-password')

    let Iskt =  false
    if (showAndHideAgainpasword) {
        showAndHideAgainpasword.onclick = () => {
            Iskt = !Iskt
            showAndHideAgainpasword.classList.toggle('active', Iskt)

            Iskt ? inPutPassword_Again.type = "text"
                : inPutPassword_Again.type = "password"

        }
    }
}
showAndHideAgainpasword()

// an hien form
function ShowAndHideForm(){
   const IconUse = $('.header__navbar--item--icon--login')
    const blur = $('.blur')
   const form_register = $("#form-register")
   const form_login = $("#form-login")
   const btn_noAccount = $(".btn-noAccount")
   const btnbackto = $(".back-to-page-login")

   function HideRegisterAndShowLogin(){
    form_login.style= "display: block"
    form_register.style= "display:none"
   }
   if(IconUse)
   {
       IconUse.onclick = () => {
           if (blur) {
               blur.classList.add("active")
               HideRegisterAndShowLogin()
           }
        }
   }
    if (blur) {
        blur.onclick = () => {
            blur.classList.remove("active")
        }
    }
    if (btnbackto) {
        btnbackto.onclick = () => {
            HideRegisterAndShowLogin()
        }
    }
    if (btn_noAccount) {
        btn_noAccount.onclick = () => {
            form_login.style = "display: none"
            form_register.style = "display:block"
        }
    }
   // dung noi bot
    if (form_register) {
        form_register.onclick = (e) => {
            e.stopPropagation()
        }
    }
    if (form_login) {
        form_login.onclick = (e) => {
            e.stopPropagation()
        }
    }
}

ShowAndHideForm()

// an hien thong bao 
function showAndHideNotifications(){
    const box_chevron_down = $(".box-chevron-down")
    const information_user= $(".information-user")

if(box_chevron_down )
{
    let isplay = false
    document.onclick=(e)=>{
        if(!e.target.closest(".information-user") && !e.target.closest(".box-chevron-down"))
        {
           
                information_user.style.display="none"
                box_chevron_down.style.transform = "rotate(180deg)"
             
        } 
        else
        {
            isplay = !isplay
            if(isplay)
            {
                information_user.style.display="block"
                box_chevron_down.style.transform = "rotate(360deg)"
            }
            else
            {
                information_user.style.display="none"
                box_chevron_down.style.transform = "rotate(180deg)"
            }
        }
    }
    
}
     
}
showAndHideNotifications()



const domtime = $("#DATE_DH")
const dominputdate = $("#form_tk_time input")
const dombtn = $("#btn-loc_tk-time")

if (domtime && dominputdate) {
    domtime.onchange = (e) => {
        dominputdate.value = e.target.value;
        dombtn.click();
    }
}

// JS Search
const domSearch = $(".seaech-btn")
const lableSearch = $(".header__navbar--search label")
const inputSearch = $(".search--input")
window.onclick = (e) => {

    if (!(e.target.closest(".seaech-btn") ||
        e.target.closest(".search--input") ||
        e.target.closest(".header__navbar--search label"))
    ) {
        domSearch.style.display = "none"
        lableSearch.style.display = "flex"
        inputSearch.style.width = "0px"
        inputSearch.style.padding = "0px"
    }
    else {
        domSearch.style.display = "block"
        lableSearch.style.display = "none"
        inputSearch.style.width = "200px"
        inputSearch.style.padding = "8px"
    }
}

function stopClick2(e) {
    e.preventDefault();
}
function sopsubmit(e) {
    e.preventDefault();
}
const hourStart = $("#hourtStart_DH")
const minuteStart = $("#minute_DH")
let DOMdate = $('#DATE_DH')
let spanmessage = $(".spanmessage")
let btn_xacnhandh2 = $('.btn-xacnhandh')
let today = new Date()
let date = `${today.getFullYear()}-${(today.getMonth() + 1) < 10 ? `0${(today.getMonth() + 1)}` : `${(today.getMonth() + 1)}`}-${today.getDate() < 10 ? `0${today.getDate()}` : `${today.getDate()}` }`;
let dateNow = new Date(date)
let mesageerror = $('.mesageerror')
let group_time_EndTime = $('.group-time-EndTime')
function KiemTraTime(){
    let sumhours = parseInt(hourStart.value) + '' + minuteStart.value
    let timeNow = parseInt(today.getMinutes()) < 10 ?
        `${today.getHours()}0${today.getMinutes()}` :
        `${today.getHours()}${today.getMinutes()}`


    if (parseInt(timeNow) > parseInt(sumhours)) {
        btn_xacnhandh2.addEventListener('click', sopsubmit);
        hourStart.style = 'border: 2px solid red'
        minuteStart.style = 'border: 2px solid red'
        mesageerror.innerText ="Giờ không hợp lệ !"
        console.log(timeNow)
        group_time_EndTime.style = "margin-top:9px";
    }
    else {
        btn_xacnhandh2.removeEventListener('click', sopsubmit);
        hourStart.style = 'border: none'
        minuteStart.style = 'border: none'
        console.log(timeNow)
        mesageerror.innerText = ""
        group_time_EndTime.style = "margin-top:20px";
    }
}

if (hourStart) {
    let dateInput = new Date(DOMdate.value)
    if (dateInput.getTime() == dateNow.getTime()) {
        KiemTraTime()
        
    }
    
    hourStart.onchange = () => {
       
        if (dateInput.getTime() == dateNow.getTime()) {
            KiemTraTime()
          
        }
    }
    minuteStart.onchange = () => {
        if (dateInput.getTime() == dateNow.getTime()) {
            KiemTraTime()
           
        }
    }
}

if (DOMdate && btn_xacnhandh2) {
    let dateInput = new Date(DOMdate.value)

    if (dateInput.getTime() >= dateNow.getTime() || DOMdate.value.trim()== "") {
        btn_xacnhandh2.removeEventListener("click", stopClick2)
    }
    else {
        btn_xacnhandh2.addEventListener("click", stopClick2)
        DOMdate.style = "border: 2px solid red";
        spanmessage.innerText = "vui lòng chọn ngày hợp lệ";
    }
}