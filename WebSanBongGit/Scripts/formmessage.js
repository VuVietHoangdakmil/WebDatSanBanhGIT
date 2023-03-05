export default message
const $10 = document.querySelector.bind(document);
 function message ({title,type, font_size,timeremove}){
    const root = $10('#form_mesage')
    if(root){
        const div =   document.createElement('div')
       
        const types = {
            error: 'fa-exclamation-triangle',
            warring :'fa-exclamation-triangle',
            success: 'fa-check'
        }
        const typev = types[type]

        div.innerHTML = `<div class="form_mesagecon ${type} ">
        <i class="fas ${typev}"> </i>
        <span style="font-size:${font_size};">${title}</span>
        </div>`
        root.appendChild(div);
        div.style.transition= "all 0.4s linear" 
            const hidendiv = setTimeout(()=>{
                div.style.transform= "translateX(-340px)" 
            },timeremove)
            const removediv = setTimeout(()=>{
                div.style.display= "none" 
            },timeremove + 500)
            div.onclick =()=>{
                div.style.transform= "translateX(-350px)";
                const removediv = setTimeout(()=>{
                    div.style.display= "none" 
                },500)
            }
    }
   
}

 