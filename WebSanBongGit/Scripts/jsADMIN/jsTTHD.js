const modal_edit = document.querySelector(".modal-edit.DDH");
const modal_edit_context = document.querySelector(".modal-edit-context")
var L = document.querySelector.bind(document)

$(document).on('click', '.table-title', function () {
    console.log("hoang")
    load()
})


function load() {
    $.ajax({
        url: '/adminLayout/DsDonDatSan',
        type: 'get',
        success: function (data) {
            if (data.code == 200) {
                $(".col-item").innerHTML = "";
                var ds = data.dsDon.map(function (item) {

                    return `  <div class="row no-gutters table-user-content">
                    <div class="col l-11tb">
                        <div class="table-col-user">
                            ${item.SDT_KH}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                           ${formatDate(cutDATE(item.NGAY_LAP_PHIEU))}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                             ${formatDate(cutDATE(item.NGAY_DA))}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                             ${item.TEN_SAN}

                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">

                             ${formatTime(cutDATE(item.GIO_BD))}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                             ${formatTime(cutDATE(item.GIO_KT))}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                             ${addCommas(item.GIA_SAN)} đ
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                            ${item.TONG_GIO_THUE}
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">
                            ${addCommas(item.TONG_TIEN)} đ
                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user">

                                <div style="color: red;" class="${item.TINH_TRANG_DON == null ? '' : 'activeNone'}">
                                    <span>Chưa đá</span>
                                    <i class="fas fa-exclamation-circle"></i>
                                </div>
                                <div style="color: green"  class="${item.TINH_TRANG_DON == true ? '' : 'activeNone'}">
                                    <span>Đá xong</span>
                                    <i class="fas fa-check-circle"></i>
                                </div>

                                <div style="color:gold"  class="${item.TINH_TRANG_DON == false ? '' : 'activeNone'}">
                                    <span>Đang đá</span>
                                    <i class="fas fa-exclamation-circle"></i>
                                </div>


                        </div>
                    </div>
                    <div class="col l-11tb">
                        <div class="table-col-user-setting">
                            <a >
                                <i class="fas fa-trash-alt"></i>
                            </a>
                            <a class="domAlink" id="${item.MA_DS}" onclick=DETAIL(${item.MA_DS}) )>
                                <i class="far fa-file-alt" ></i>
                            </a>
                            <a >
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    </div>
                </div>`
                })

                $(".col-item").innerHTML = ds.join("")
            }
        }
    })
}

function DETAIL(id) {
    $.ajax({
        url: '/adminLayout/detailDDS',
        type: 'get',
        date: {
            id: id
        },
        success: function (data) {
            if (data.code == 200) {
                $("SDT-DDS").val = data.SDT_KH;
                modal_edit.style.display = "block";
            }
        }
    })
}
//setTimeout(() => {
//    var fa_file_alt = document.querySelectorAll('.domAlink')
//    fa_file_alt.forEach(item => {
//        item.onclick = (e) => {

//            $.ajax({
//                url: '/adminLayout/detailDDS',
//                type: 'get',
//                date: {
//                    id: parseInt(item.getAttribute('id'))
//                },
//                success: function (data) {
//                    if (data.code == 200) {
//                        $("SDT-DDS").val = data.SDT_KH;
//                        modal_edit.style.display = "block";
//                    }
//                }
//            })
//        }
//    })

//}, 500)



// hide modal
modal_edit.onclick = (e) => {
    modal_edit.style.display = "none";
}
// dung noi bot
modal_edit_context.addEventListener("click", (e) => {
    e.stopPropagation();
})

const addCommas = value => {
    const stringFormat = `${value}`;
    const x = stringFormat.split('.');
    let x1 = x[0];
    const x2 = x.length > 1 ? `.${x[1]}` : '';
    const regex = /(\d+)(\d{3})/;
    while (regex.test(x1)) {
        x1 = x1.replace(regex, '$1,$2');
    }
    return x1 + x2;
};

function cutDATE(cut) {
    var cutstring = cut.slice(6, 19)
    var date2 = parseInt(cutstring)
    var date = new Date(date2)
    return date
}
function formatTime(timefomat) {
    var time
    if (timefomat.getMinutes() < 10 && timefomat.getHours() < 10) {
        time = `0${timefomat.getHours()}:0${timefomat.getMinutes()}`
    }
    else if (timefomat.getMinutes() < 10) {

        time = `${timefomat.getHours()}:0${timefomat.getMinutes()}`

    }
    else if (timefomat.getHours() < 10) {

        time = `0${timefomat.getHours()}:${timefomat.getMinutes()}`
    }
    else {
        time = `${timefomat.getHours()}:${timefomat.getMinutes()}`
    }
    return time
}
function formatDate(dateformar) {
    var date = dateformar.getDate() + '/' + (dateformar.getMonth() + 1) + '/' + dateformar.getFullYear();
    return date
}

