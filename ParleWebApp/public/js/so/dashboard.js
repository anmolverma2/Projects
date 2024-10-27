var DSREnrollmentOption = {
    colors: ['#FD8080', '#94DAFB', '#26E7A6', '#FEBC3B'],
    series: [44, 55, 13, 34],
    chart: {
        type: 'donut',
        height: 170
    },
    labels: ['Rejected', 'Approved', 'Pending for Enrollment', 'Pending for Verfication'],
    dataLabels: {
        enabled: false,
    },
    legend: {
        show: false
    },
    plotOptions: {
        pie: {
            donut: {
                size: '70%',
                labels: {
                    show: true,
                    name: {
                        show: true,
                        fontSize: '24px',
                        fontFamily: 'Arial',
                        fontWeight: 'bold',
                        color: '#888',
                        offsetY: -10,
                        // formatter: function (val) {
                        //   return 'Total';
                        // },
                    },
                    value: {
                        show: true,
                        fontSize: '26px',
                        fontFamily: 'Arial',
                        fontWeight: 'bold',
                        color: '#333',
                        offsetY: 0,
                        formatter: function(val) {
                            return val + '%';
                        },
                    },
                    total: {
                        show: true,
                        showAlways: true,
                        fontSize: '1.2rem',
                        fontFamily: 'Arial',
                        fontWeight: 'normal',
                        color: '#888',
                        // label: 'Total',
                        formatter: function(w) {
                            return w.globals.seriesTotals.reduce(function(a, b) {
                                return a + b;
                            }, 0);
                        },
                    },
                },
            },
        },
    },
    annotations: {
        position: 'front',
        yaxis: [{
            y: '50%',
            borderWidth: 0,
            borderColor: '#ccc',
            label: {
                show: true,
                text: '',
                style: {
                    fontSize: '36px',
                    fontFamily: 'Arial',
                    fontWeight: 'bold',
                    color: '#333',
                },
            },
        }, ],
    },
    responsive: [{
        breakpoint: 768,
        options: {
            plotOptions: {
                pie: {
                    donut: {
                        size: '70%',

                        labels: {
                            show: true,
                            name: {
                                show: true,
                                fontSize: '16px',
                                fontFamily: 'Arial',
                                fontWeight: 'bold',
                                color: '#888',
                                offsetY: -3,
                            },
                            value: {
                                show: true,
                                fontSize: '20px',
                                fontFamily: 'Arial',
                                fontWeight: 'bold',
                                color: '#333',
                                offsetY: 3,
                                formatter: function(val) {
                                    return val + '%';
                                },
                            },
                            total: {
                                show: true,
                                showAlways: true,
                                fontSize: '1.2rem',
                                fontFamily: 'Arial',
                                fontWeight: 'normal',
                                color: '#888',
                                label: 'Total',
                                formatter: function(w) {
                                    return w.globals.seriesTotals.reduce(function(a, b) {
                                        return a + b;
                                    }, 0);
                                },
                            },
                        },
                    },
                },
            },
        },
    }]

};

var DSREnrollment = new ApexCharts(document.querySelector('#dsr-enrollment'), DSREnrollmentOption);
DSREnrollment.render();

var EnrollmentOption = {
    series: [44, 55, 13, 34],
    colors: ['#FD8080', '#94DAFB', '#26E7A6', '#FEBC3B'],
    chart: {
        type: 'donut',
        height: 170
    },
    labels: ['Approved', 'Rejected', 'Pending for Enrollment', 'Pending for Verfication'],
    dataLabels: {
        enabled: false,
    },
    legend: {
        show: false
    },
    plotOptions: {
        pie: {
            donut: {
                size: '70%',
                labels: {
                    show: true,
                    name: {
                        show: true,
                        fontSize: '24px',
                        fontFamily: 'Arial',
                        fontWeight: 'bold',
                        color: '#888',
                        offsetY: -10,
                        // formatter: function (val) {
                        //   return 'Total';
                        // },
                    },
                    value: {
                        show: true,
                        fontSize: '26px',
                        fontFamily: 'Arial',
                        fontWeight: 'bold',
                        color: '#333',
                        offsetY: 0,
                        formatter: function(val) {
                            return val + '%';
                        },
                    },
                    total: {
                        show: true,
                        showAlways: true,
                        fontSize: '18px',
                        fontFamily: 'Arial',
                        fontWeight: 'normal',
                        color: '#888',
                        // label: 'Total',
                        formatter: function(w) {
                            return w.globals.seriesTotals.reduce(function(a, b) {
                                return a + b;
                            }, 0);
                        },
                    },
                },
            },
        },
    },
    annotations: {
        position: 'front',
        yaxis: [{
            y: '50%',
            borderWidth: 0,
            borderColor: '#ccc',
            label: {
                show: true,
                text: '',
                style: {
                    fontSize: '36px',
                    fontFamily: 'Arial',
                    fontWeight: 'bold',
                    color: '#333',
                },
            },
        }, ],
    },
    responsive: [{
        breakpoint: 768,
        options: {
            plotOptions: {
                pie: {
                    donut: {
                        size: '70%',

                        labels: {
                            show: true,
                            name: {
                                show: true,
                                fontSize: '16px',
                                fontFamily: 'Arial',
                                fontWeight: 'bold',
                                color: '#888',
                                offsetY: -3,
                                // formatter: function (val) {
                                //   return 'Total';
                                // },
                            },
                            value: {
                                show: true,
                                fontSize: '20px',
                                fontFamily: 'Arial',
                                fontWeight: 'bold',
                                color: '#333',
                                offsetY: 3,
                                formatter: function(val) {
                                    return val + '%';
                                },
                            },
                            total: {
                                show: true,
                                showAlways: true,
                                fontSize: '22px',
                                fontFamily: 'Arial',
                                fontWeight: 'normal',
                                color: '#888',
                                label: 'Total',
                                formatter: function(w) {
                                    return w.globals.seriesTotals.reduce(function(a, b) {
                                        return a + b;
                                    }, 0);
                                },
                            },
                        },
                    },
                },
            },
        },
    }]

};

var Enrollment = new ApexCharts(document.querySelector('#enrollment'), EnrollmentOption);
Enrollment.render();


var DSRPaymentOption = {
    colors: ['#FD8080', '#94DAFB', '#26E7A6', '#FEBC3B'],
    series: [{
        data: [21, 22, 10, 28]
    }],
    chart: {
        height: 170,
        type: 'bar',
        toolbar: {
            show: false
        }
    },
    // colors: colors,
    plotOptions: {
        bar: {
            columnWidth: '30%',
            distributed: true,
        }
    },
    dataLabels: {
        enabled: false
    },
    legend: {
        show: false
    },
    grid: {
        show: false
    },
    yaxis: {
        labels: {
            show: false,
        },
    },
    xaxis: {

        labels: {
            show: false,
        },
        axisTicks: {
            show: false
        }

    }
};

var DSRPayment = new ApexCharts(document.querySelector("#dsr-payment"), DSRPaymentOption);
DSRPayment.render();


var PaymentOption = {
    colors: ['#FD8080', '#94DAFB', '#26E7A6', '#FEBC3B'],
    series: [{
        data: [21, 22, 10, 28]
    }],
    chart: {
        height: 170,
        type: 'bar',
        toolbar: {
            show: false
        },

    },
    // colors: colors,
    plotOptions: {
        bar: {
            columnWidth: '30%',
            distributed: true,
        },

    },
    dataLabels: {
        enabled: false
    },
    legend: {
        show: false
    },
    grid: {
        show: false
    },
    yaxis: {
        labels: {
            show: false,
        },
    },
    xaxis: {

        labels: {
            show: false,
        },
        axisTicks: {
            show: false
        }
    }
};

var Payment = new ApexCharts(document.querySelector("#payment"), PaymentOption);
Payment.render();




salesmanEnrollment = () => {
    location.href = "dsr/dsr.html";
}
salesmanPayment = () => {
    location.href = "salesman-payment.html"
}
retailerEnrollment = () => {
    location.href = "store/store.html";
}
retailerPayment = () => {
    location.href = "retailer-payment.html"
}