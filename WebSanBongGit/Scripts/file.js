// js nut chose file
const $2 = document.querySelector.bind(document);


const ChooseFlie = $2('.chosseFile')
const FILEIMG_iNf = $2('#FILEIMG_iNf')
 if(ChooseFlie){
     ChooseFlie.onclick = ()=>{
         FILEIMG_iNf.click()
     }
 }