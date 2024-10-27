// radio otption
$(".gst-detail,.pan-detail").addClass('d-none');
$('input[type="radio"]').click(function() {
    var inputValue = $(this).attr("value");
    if (inputValue == "aadhar") {
        $("#proof-upload").removeClass('d-none');
        $("#proof-upload > div:nth-child(2)").removeClass('d-none');
    } else if (inputValue == "voter") {
        $("#proof-upload").removeClass('d-none');
        $("#proof-upload > div:nth-child(2)").removeClass('d-none');
    } else if (inputValue == "driving") {
        $("#proof-upload").removeClass('d-none');
        $("#proof-upload > div:nth-child(2)").addClass('d-none');
    }

    if (inputValue == "gstyes") {
        $(".gst-detail").removeClass('d-none');
        $(".pan-detail").addClass('d-none');
    } else if (inputValue == "gstno") {
        $(".gst-detail").addClass('d-none');
        $(".pan-detail").removeClass('d-none');
    }
});
//upload document
function previewFile(input) {
    let current = input.previousElementSibling;
    let preview = input.nextElementSibling;
    let file = input.files[0];
    let filename = input.files[0].name;
    let fileid = input.nextElementSibling.getAttribute("id").substr(5);
    let deleteimage = input.nextElementSibling.nextElementSibling;

    let reader = new FileReader();
    reader.onloadend = function() {
        current.classList.add('d-none');
        preview.classList.remove('d-none');
        preview.nextElementSibling.classList.remove('d-none');
        preview.src = reader.result;
        $("#filename" + fileid).text(filename)
    }
    if (file) {
        reader.readAsDataURL(file);
    } else {
        preview.src = "";
    }

    $(deleteimage).on("click", function() {
        let imageID = $(this).prev().prev().attr("id").substr(7);
        let fileID = imageID.substr(5);
        console.log(fileID);
        $("#" + imageID).attr("src", "");
        $("#filename" + fileID).text(" ");
        current.classList.remove('d-none');
        preview.classList.add('d-none');
        preview.nextElementSibling.classList.add('d-none');
    });
}
// // Loader
$('.next-btn').click(function() {
    //$('.loaderdiv').toggleClass('show');
    $('.loaderdiv').toggleClass('show');
    $('body').addClass('overflow-hidden');
    //location.href = "personal-detail.html";
});