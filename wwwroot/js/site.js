// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function buy(carId) {
    $.ajax({
        url: '../api/Car/BuyCar',
        type: 'POST',
        data: JSON.stringify(carId), // replace carId with the actual id of the car
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            location.reload();
        },
        error: function (data) {
            // alert("Error buying car!");
            console.log(data);
            console.log(data.responseText);
        }
    });
}

function removeFromStore(carId) {
    
    $.ajax({
            url: '../api/Store/RemoveFromStore',
            type: 'POST',
        data: JSON.stringify(carId), // replace carId with the actual id of the car
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            location.reload();
        },
        error: function (data) {
            // alert("Error buying car!");
            console.log(data.responseText);
        }
    });
}