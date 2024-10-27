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

});
//upload document
function previewFile(input) {
    debugger;
    let current = input.previousElementSibling;
    let preview = input.nextElementSibling;
    let file = input.files[0];
    let filename = input.files[0].name;
    let fileid = input.nextElementSibling.getAttribute("id").substr(5);
    let deleteimage = input.nextElementSibling.nextElementSibling;
    $('#image'+fileid).val(filename);

    let reader = new FileReader();
    if (filename.split(".")[1].toLowerCase() == "jpeg" || filename.split(".")[1].toLowerCase() == "jpg" || filename.split(".")[1].toLowerCase() == "png") {
        reader.onloadend = function() {
            current.classList.add('d-none');
            preview.classList.remove('d-none');
            preview.nextElementSibling.classList.remove('d-none');
            preview.src = reader.result;
            $("#filename" + fileid).text(filename)
        }
    } else {
        Swal.fire({
            icon: 'error',
            text: ' kindly upload image in jpg,jpeg or png format',
        })
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
        $('#image'+fileID).val('');
        $("#" + imageID).attr("src", "");
        $("#filename" + fileID).text(" ");
        current.classList.remove('d-none');
        preview.classList.add('d-none');
        preview.nextElementSibling.classList.add('d-none');
    });
}
