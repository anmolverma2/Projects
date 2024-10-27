// Filter by clicking on card
filterSelection("all")

function filterSelection(value) {
    let elements = document.querySelectorAll(".box-shadow");
    $('#search-input').val('')
    elements.forEach((element) => {
        if (value == "all") {
            element.parentElement.classList.remove('d-none');
        } else {
            if (element.classList.contains(value)) {
                element.parentElement.classList.remove('d-none');

            } else {
                element.parentElement.classList.add('d-none');
            }
        }
    });
}

//search by input field
function searchProgram() {
    var searchValue = $('#search-input').val().toLowerCase();
    $(".filterContainer").filter(function() {
        if ($(this).text().toLowerCase().indexOf(searchValue) > 0) {
            $(this).removeClass('d-none');
        } else {
            $(this).addClass('d-none');
        }
    });
}
$("#search").on("click", function() {
    if ($('#search-input').val() == '' || $('#search-input').val() == null) {
        $(".filterContainer").show();
    } else {
        searchProgram();
    }
});


pending = () => {
    Swal.fire({
        icon: 'info',
        title: 'KYC is under verfication',
        confirmButtonColor: "#2a69b2",
        confirmButtonText: 'Okay',
    })
}

redirectToUploadDocument = () => {
    location.href = "../kyc/upload-document.html";
}
redirectToApproval = () => {
    location.href = "../kyc/approval.html";
}
redirectToRejected = () => {
    location.href = "../kyc/rejected.html";
}

sendlink = () => {
    Swal.fire({
        icon: 'success',
        title: "<h4 class='text-success'>Successfully</h4>",
        text: 'KYC Link has been sent',
        confirmButtonColor: "#2a69b2",
        confirmButtonText: 'Okay',

    })
}

// slider filter in mobile view
$('.multiple-items').slick({
    infinite: false,
    // autoplay: true,
    slidesToShow: 3,
    slidesToScroll: 1,
    arrows: true,
    variableWidth: true,
    adaptiveHeight: true,
    useTransform: true,
    touchMove: false,
    pauseOnHover: false,
    responsive: [{
            breakpoint: 1024,
            settings: {
                infinite: false,
                slidesToShow: 3,
                slidesToScroll: 3,
                arrows: true,
                useTransform: true,
                touchMove: false,
                pauseOnHover: false,
                cssEase: "linear",

            }
        }, {
            breakpoint: 600,
            settings: {
                infinite: false,
                slidesToShow: 2,
                slidesToScroll: 2,
                arrows: true,
                useTransform: true,
                touchMove: false,
                pauseOnHover: false,
                cssEase: "linear",
            }
        }, {
            breakpoint: 480,
            settings: {
                infinite: false,
                slidesToShow: 2,
                arrows: false,
                slidesToScroll: 1,
                useTransform: true,
                touchMove: false,
                pauseOnHover: false,
                cssEase: "linear",
            }
        }

    ]

});
$(".slick-slide").click(function() {
    $('.active-slick-slide').removeClass("active-slick-slide");
    $(this).addClass("active-slick-slide");
})