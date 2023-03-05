
import Message from './formmessage.js'
const $B = document.querySelector.bind(document)
const $BaLL = document.querySelectorAll.bind(document)
const hourStart = $B("#hourtStart_DH")
const hourEnd = $B("#hourtEnd_DH")
const minuteEnd_DH = $B("#minuteEnd_DH")
const minuteStart = $B("#minute_DH")
const btn_xacnhandh = $B(".btn-xacnhandh")
const listtimeFrame = $BaLL("#time-frame .u li")
/*Message({ title: "Vui Lòng Chọn Thời Gian Đặt Sân ", type: "warring", font_size: "15px", timeremove: 6000 })*/
if (hourEnd && hourStart
    && minuteEnd_DH
    && minuteStart
    && btn_xacnhandh) {
    let valuestart = hourStart.value;
    let valueEnd = hourEnd.value;
    let valueminuteStart = minuteStart.value;
    let valueminuteEnd_DH = minuteEnd_DH.value;
    let sumtimeStart = valuestart + valueminuteStart
    let sumtimeEND = valueEnd + valueminuteEnd_DH

    if (!(parseInt(sumtimeEND) > parseInt(sumtimeStart))) {
        btn_xacnhandh.disabled = "disabled"
    }

    function ktTimeMaxMin(valueHourStart, valueminuteStart1, valueHourEnd, valueminuteEnd1) {
        let sumtimeStart = valueHourStart.trim() + valueminuteStart1.trim()
        let sumtimeEND = valueHourEnd.trim() + valueminuteEnd1.trim()

        // su ly gioKT > gioBD
        if (parseInt(sumtimeEND) > parseInt(sumtimeStart)) {
            btn_xacnhandh.removeAttribute("disabled")
            btn_xacnhandh.removeEventListener("click", stopClick)
        }
        else {
            btn_xacnhandh.disabled = "disabled"
            Message({ title: "Giờ Bắt Đầu Phải Bé Hơn Giờ Kết Thúc", type: "warring", font_size: "15px", timeremove: 4000 })
        }
    }

    function cutTimeStartINT(h) {
        let startTime = h.slice(0, 2) + h.slice(3, 5)
        return parseInt(startTime)
    }

    function cutTimeEndINT(h) {
        let endTime = h.slice(8, 10) + h.slice(11, 13)
        return parseInt(endTime)
    }

    function stopClick(e) {
        e.preventDefault();
    }

    function areTwoDateTimeRangesOverlapping(incommingDateTimeRange, timeliststart, timelistEnd) {
        return incommingDateTimeRange.start < timelistEnd && incommingDateTimeRange.end > timeliststart
    }

    function areManyDateTimeRangesOverlapping(incommingDateTimeRange, existingDateTimeRanges) {
    
        let is
        for (let item of existingDateTimeRanges) {
            let starttime = cutTimeStartINT(item.innerText)
            let EndTime = cutTimeEndINT(item.innerText)
            if (areTwoDateTimeRangesOverlapping(incommingDateTimeRange, starttime, EndTime)) {
                is = true;
                break;
            }
            else {
                is = false;
            }
        }
        return is;
    }
   
    function ktTime(valueHourStart, valueMinuteStart, valueHourEnd, valueMinuteEnd) {
        let sumtimeStartINT = valueHourStart.trim() + valueMinuteStart.trim()
        let sumtimeENDINT = valueHourEnd.trim() + valueMinuteEnd.trim()
        let sumALLhourinput = {
            start: parseInt(sumtimeStartINT),
            end: parseInt(sumtimeENDINT)
        }

        let isTime = areManyDateTimeRangesOverlapping(sumALLhourinput, listtimeFrame)

        if (isTime) {
            btn_xacnhandh.addEventListener("click", stopClick)
            Message({ title: "Giờ đã có người đặt", type: "error", font_size: "20px", timeremove: 5000 })
        }


    }
    minuteEnd_DH.oninput = function (e) {

        valueminuteEnd_DH = e.target.value
        ktTimeMaxMin(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
        ktTime(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)

    }
    hourEnd.oninput = function (e) {
        valueEnd = e.target.value
        ktTimeMaxMin(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
        ktTime(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
    }

    minuteStart.oninput = function (e) {
        valueminuteStart = e.target.value
        ktTimeMaxMin(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
        ktTime(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
    }

    hourStart.oninput = function (e) {
        valuestart = e.target.value
        ktTimeMaxMin(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
        ktTime(valuestart, valueminuteStart, valueEnd, valueminuteEnd_DH)
    }
}