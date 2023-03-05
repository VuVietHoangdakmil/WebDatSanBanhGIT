const $ = document.querySelector.bind(document);

const domSANBONG =  $(".licha.click1")
const domTIME =  $(".licha.click2")
const domSANBONGUL =  $(".list_manage_san_child")
const domTIMEUL =  $(".list_manage_time_child")
const domSANBONGdown= $(".licha.click1 .fa-caret-left")
const domTIMEdown = $(".licha.click2 .fa-caret-left")

let IsSanBONG = false
let IsTIME = false

function clicknavagation(){
    IsSanBONG = !IsSanBONG
    if(IsSanBONG){
        domSANBONGUL.style.height ="166px"
        domSANBONGdown.style.transform = "rotate(-90deg)"
    }else
    {
        domSANBONGUL.style.height ="0"
        domSANBONGdown.style.transform = "rotate(0deg)"
    }
}
function click2(){
    IsTIME = !IsTIME
    if(IsTIME){
        domTIMEUL.style.height ="94px"
        domTIMEdown.style.transform = "rotate(-90deg)"
    }else
    {
        domTIMEUL.style.height ="0"
        domTIMEdown.style.transform = "rotate(0deg)"
    }
}

//click nvagations
if(domSANBONG&&domTIME&&domSANBONGUL&&domTIMEUL){
    domSANBONG.addEventListener("click",clicknavagation)
    domTIME.addEventListener("click",click2)
}

// click namespace
const domnameICONSETTING = $(".wrap-name-icon-setting")
const domnameICON = $(".wrap-name-icon")

if(domnameICONSETTING && domnameICON)
{
    let isIconSETTING = false;
    domnameICON.onclick = ()=>{
        isIconSETTING = !isIconSETTING;
        
        if(isIconSETTING){
            domnameICONSETTING.style.display = 'block'
        }
        else
        {
            domnameICONSETTING.style.display = 'none'
        }
    }
     window.onclick = (e)=>{
      
        if(!e.target.closest('.wrap-name-icon') )
        { 
            isIconSETTING=false;
            domnameICONSETTING.style.display = 'none'
        }
     }
}

// click MENU
const domMenu= $(".nav-bar_menu_left > i")
const domcol1 = $(".col.l-2.editcol1")
const domcol2 = $(".col.l-10.editcol2")

let iscollocal = JSON.parse(localStorage.getItem("IScol")) || false
if (iscollocal) {
    domcol1.classList.remove("l-2")
    domcol1.classList.add("lc1")

    domcol2.classList.remove("l-10")
    domcol2.classList.add("lc11")

   
}
else {
    domcol1.classList.remove("lc1")
    domcol1.classList.add("l-2")

    domcol2.classList.remove("lc11")
    domcol2.classList.add("l-10")

    domSANBONG.addEventListener("click", clicknavagation)
    domTIME.addEventListener("click", click2)
}

domMenu.onclick = () => {
    localStorage.setItem("IScol", JSON.stringify(!JSON.parse(localStorage.getItem("IScol"))))
    iscollocal = JSON.parse(localStorage.getItem("IScol"))
    if (JSON.parse(localStorage.getItem("IScol"))) {
        domcol1.classList.remove("l-2")
        domcol1.classList.add("lc1")

        domcol2.classList.remove("l-10")
        domcol2.classList.add("lc11")

    }
    else {
        domcol1.classList.remove("lc1")
        domcol1.classList.add("l-2")

        domcol2.classList.remove("lc11")
        domcol2.classList.add("l-10")

        domSANBONG.addEventListener("click", clicknavagation)
        domTIME.addEventListener("click", click2)
    }
}
// kt item sideBar Active
function ktItemSideBar_Active() {
    const listLicha = document.querySelectorAll('.licha');
    const IstableUser = $('#ISTTKH')
    const IstableTT_DS = $("#IS_TT-DDS")
    const IsmainPanel = $('.main-panel')
    const IsremoveUser = $('.removeUser')
    const IS_QLTimeSan = $("#IS_QLTimeSan")
    const activeOpenTTS = $("#activeOpenTTS")
    const ISList_SAN10 = $("#ISList_SAN10")
    const IS_THONGke = $('#IS_THONGke')
    const TABLE_TT_TK_ADM = $("#TABLE_TT_TK_ADM")
    const Is_ListLOAIsAN = $('#IsListLoaiSan')
    const LoaiSanAC = $('#activeOpenLISTLOAISAN')

    if (Is_ListLOAIsAN) {
        domSANBONG.click();
        LoaiSanAC.style.color = "#04b0ce";
        listLicha[3].classList.add('active')
    }
    if (TABLE_TT_TK_ADM) {
        listLicha[1].classList.add('active')
    }
    if (IstableUser ) {
        listLicha[2].classList.add('active')
    }
    if (IsmainPanel) {
        listLicha[0].classList.add('active')
    }
    if (IstableTT_DS) {
        domSANBONG.click();
        $('#activeOpenDDS').style.color = "#04b0ce";
        listLicha[3].classList.add('active')
    }
    if (IS_QLTimeSan) {
        domSANBONG.click();
        activeOpenTTS.style.color = "#04b0ce";
        listLicha[3].classList.add('active')
    }
    if (ISList_SAN10) {
        domSANBONG.click();
        $('#activeOpenLISTSAN').style.color = "#04b0ce";
        listLicha[3].classList.add('active')
    }
    if (IS_THONGke) {
        listLicha[5].classList.add('active')
    }
}
ktItemSideBar_Active()