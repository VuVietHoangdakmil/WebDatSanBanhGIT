const S = document.querySelector.bind(document)
const SS = document.querySelectorAll.bind(document)

let today = new Date()
let date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
let dateNow = new Date(date)

let isNgayHienHanh = true
let isGioHienHang = true
let isSoSanhNgay = true
let isSoSangTime = true

const Dom_inputStartDate = S("#TimKiemSanMain_col_left_action_type-StartDate")
const Dom_inputEndDate = S("#TimKiemSanMain_col_left_action_type-endDate")
const Dom_inputStartTime = S("#TimKiemSanMain_col_left_action_type-StartTime")
const Dom_inputEndTime = S("#TimKiemSanMain_col_left_action_type-endTime")
const Dom_optionStartTime = S(".opion_StartTime")
const Dom_optionEndTime = S(".opion_EndTime")
const Dom_opion_StartTime_hours = S("#opion_StartTime_hours")
const Dom_opion_StartTime_minute = S("#opion_StartTime_minute")
const Dom_opion_endTime_hours = S("#opion_EndTime_hours")
const Dom_opion_endTime_minute = S("#opion_EndTime_minute")
const DomDateStart = S("#TimKiemSanMain_col_left_action_type-StartDate")
const DomDateEnd = S("#TimKiemSanMain_col_left_action_type-endDate")
const btn_TimKiem = S('.btn_timKiem_sanTrong')

////////////////////////////////////////////////////////////////////////////////
Dom_opion_StartTime_hours.onchange = () => {

    const valueInputStartTime = `${Dom_opion_StartTime_hours.value}:${Dom_opion_StartTime_minute.value}`
    Dom_inputStartTime.value = valueInputStartTime


}
Dom_opion_StartTime_minute.onchange = () => {

    const valueInputStartTime = `${Dom_opion_StartTime_hours.value}:${Dom_opion_StartTime_minute.value}`
    Dom_inputStartTime.value = valueInputStartTime
}
Dom_opion_endTime_hours.onchange = () => {

    const valueInputEndTime = `${Dom_opion_endTime_hours.value}:${Dom_opion_endTime_minute.value}`
    Dom_inputEndTime.value = valueInputEndTime
}
Dom_opion_endTime_minute.onchange = () => {

    const valueInputEndTime = `${Dom_opion_endTime_hours.value}:${Dom_opion_endTime_minute.value}`
    Dom_inputEndTime.value = valueInputEndTime
}
/////////////////////////////////////////////////////////

window.onclick = (e) => {
    let IsReMoveStartOption = e.target.closest(".opion_StartTime")
    let IsReMoveStartInput = e.target.closest("#TimKiemSanMain_col_left_action_type-StartTime")
    let IsReMoveEndOption = e.target.closest(".opion_EndTime")
    let IsReMoveEndInput = e.target.closest("#TimKiemSanMain_col_left_action_type-endTime")
    if (!IsReMoveStartOption && !IsReMoveStartInput) {
        Dom_optionStartTime.style.display = "none"
    } else {
        Dom_optionStartTime.style.display = "block"
    }

    if (!IsReMoveEndOption && !IsReMoveEndInput) {
        Dom_optionEndTime.style.display = "none"
    } else {
        Dom_optionEndTime.style.display = "block"
    }

}
function KiemTraTime() {

    let sumhours = parseInt(Dom_opion_StartTime_hours.value) + '' + Dom_opion_StartTime_minute.value
    let timeNow = parseInt(today.getMinutes()) < 10 ?
        `${today.getHours()}0${today.getMinutes()}` :
        `${today.getHours()}${today.getMinutes()}`


    if (parseInt(sumhours) < parseInt(timeNow) && new Date(DomDateStart.value).getTime() == dateNow.getTime()) {

        isGioHienHang = false
    }
    else {

        isGioHienHang = true
    }

    if (isGioHienHang) {
        S('#TimKiemSanMain_col_left_action_type-StartTime ~ span').textContent = ""
        Dom_inputStartTime.style.border = "none"
    }
    else {
        S('#TimKiemSanMain_col_left_action_type-StartTime ~ span').textContent = "Chọn giờ lớn hơn hoặc bằng giờ hiện tại"
        Dom_inputStartTime.style.border = "2px solid red"
    }
    if (isGioHienHang == true && isSoSangTime == false) {
        S('#TimKiemSanMain_col_left_action_type-endTime').style.border = "2px solid red"
        S('#TimKiemSanMain_col_left_action_type-endTime ~ span').textContent = "Chọn lớn hơn giờ bắt đầu"
        S('#TimKiemSanMain_col_left_action_type-StartTime').style.border = "2px solid red"
        S('#TimKiemSanMain_col_left_action_type-StartTime ~ span').textContent = "Chọn bé hơn giờ bắt đầu"
    }
}

if (Dom_opion_StartTime_hours) {
    let value_timeKT
    let value_timeBD
    Dom_opion_StartTime_hours.oninput = () => {
        sosanhActiveTime()
        KiemTraTime()


    }
    Dom_opion_StartTime_minute.oninput = () => {
        sosanhActiveTime()
        KiemTraTime()

    }
    Dom_opion_endTime_hours.oninput = () => {

        sosanhActiveTime()
        KiemTraTime()
    }
    Dom_opion_endTime_minute.oninput = () => {
        sosanhActiveTime()
        KiemTraTime()
    }
}
function sosanhActiveTime() {
    value_timeKT = parseInt(`${Dom_opion_endTime_hours.value}${Dom_opion_endTime_minute.value}`)
    value_timeBD = parseInt(`${Dom_opion_StartTime_hours.value}${Dom_opion_StartTime_minute.value}`)
    if (value_timeBD >= value_timeKT) {

        isSoSangTime = false
        S('#TimKiemSanMain_col_left_action_type-endTime').style.border = "2px solid red"
        S('#TimKiemSanMain_col_left_action_type-endTime ~ span').textContent = "Chọn lớn hơn giờ bắt đầu"
        S('#TimKiemSanMain_col_left_action_type-StartTime').style.border = "2px solid red"
        S('#TimKiemSanMain_col_left_action_type-StartTime ~ span').textContent = "Chọn bé hơn giờ bắt đầu"
    }
    else {
        isSoSangTime = true
        S('#TimKiemSanMain_col_left_action_type-endTime').style.border = "none"
        S('#TimKiemSanMain_col_left_action_type-endTime ~ span').textContent = ""
        S('#TimKiemSanMain_col_left_action_type-StartTime').style.border = "none"
        S('#TimKiemSanMain_col_left_action_type-StartTime ~ span').textContent = ""
    }
}
if (DomDateStart) {
    let valueDomDateStartSS
    let valueDomDateEndSS
    function xulySoSanhNgat(isSoSanhNgay, DomDateStart, DomDateEnd) {
        if (isSoSanhNgay) {
            DomDateStart.style.border = "none"
            DomDateEnd.style.border = "none"
            S('#TimKiemSanMain_col_left_action_type-endDate ~ span ').textContent = ""
            S('#TimKiemSanMain_col_left_action_type-StartDate ~ span ').textContent = ""

        } else {
            DomDateStart.style.border = "2px solid  red "
            S('#TimKiemSanMain_col_left_action_type-StartDate ~ span ').textContent = "Chọn bé hơn hoặc bằng giờ bắt đầu"
            DomDateEnd.style.border = "2px solid  red "
            S('#TimKiemSanMain_col_left_action_type-endDate ~ span ').textContent = "Chọn lớn hơn hoặc bằng giờ bắt đầu"
        }
    }

    DomDateStart.oninput = () => {
        valueDomDateStartSS = new Date(DomDateStart.value)
        valueDomDateEndSS = new Date(DomDateEnd.value)

        if (new Date(DomDateStart.value).getTime() >= dateNow.getTime() || DomDateStart.value.trim() == "") {
            DomDateStart.style.border = "none";
            S("#TimKiemSanMain_col_left_action_type-StartDate ~ span").textContent = ""
            isNgayHienHanh = true
        }
        else {
            DomDateStart.style.border = "2px solid  red";
            isNgayHienHanh = false
            S("#TimKiemSanMain_col_left_action_type-StartDate ~ span").textContent = "Chọn lớn hơn hoặc bằng ngày hiện tại"
        }



        if (DomDateStart.value != "" && DomDateEnd.value != "") {
            if (valueDomDateStartSS <= valueDomDateEndSS) {
                console.log("dung")
                isSoSanhNgay = true
            } else {
                console.log("sai")
                isSoSanhNgay = false
            }
            xulySoSanhNgat(isSoSanhNgay, DomDateStart, DomDateEnd)
        }

        if (!isNgayHienHanh) {
            DomDateStart.style.border = "2px solid  red";

            S("#TimKiemSanMain_col_left_action_type-StartDate ~ span").textContent = "Chọn lớn hơn hoặc bằng ngày hiện tại"
        }
        KiemTraTime()
    }

    DomDateEnd.oninput = () => {
        valueDomDateStartSS = new Date(DomDateStart.value)
        valueDomDateEndSS = new Date(DomDateEnd.value)

        if (DomDateStart.value != "" && DomDateEnd.value != "") {
            if (valueDomDateStartSS <= valueDomDateEndSS) {
                console.log("dung")
                isSoSanhNgay = true

            } else {
                console.log("sai")
                isSoSanhNgay = false
            }
            xulySoSanhNgat(isSoSanhNgay, DomDateStart, DomDateEnd)
        }
        if (!isNgayHienHanh) {
            DomDateStart.style.border = "2px solid  red";
            S("#TimKiemSanMain_col_left_action_type-StartDate ~ span").textContent = "Chọn lớn hơn hoặc bằng ngày hiện tại"
        }
        KiemTraTime()
    }
}

btn_TimKiem.onclick = () => {
    if (Dom_inputStartDate.value == "" || Dom_inputEndDate.value == "" || Dom_inputStartTime.value == "" || Dom_inputEndTime.value == "") {
        alert("vui lòng nhập đầy đủ dữ liệu để tìm kiếm")
    }
    else {
        if (isNgayHienHanh && isSoSanhNgay && isSoSangTime && isGioHienHang) {
            alert("tim kiem thanh cong")
        }
        else {

        }
    }
}
