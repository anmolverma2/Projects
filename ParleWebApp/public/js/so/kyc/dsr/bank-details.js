$(".bank-verify").click(function() {
    $(".verified-msg").removeClass('d-none');
    timer();
});




function previewFile(input) {
    let current = input.previousElementSibling;
    let preview = input.nextElementSibling;
    let file = input.files[0];
    let filename = input.files[0].name;
    let fileid = input.nextElementSibling.getAttribute("id").substr(5);
    let deleteimage = input.nextElementSibling.nextElementSibling;

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
        $("#" + imageID).attr("src", "");
        $("#filename" + fileID).text(" ");
        current.classList.remove('d-none');
        preview.classList.add('d-none');
        preview.nextElementSibling.classList.add('d-none');
    });
}