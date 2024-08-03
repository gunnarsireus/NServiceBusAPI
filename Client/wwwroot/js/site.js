// Write your JavaScript code.
$(document).ready(() => {

  const ShowCars = () => {
    window.location = "/Car/index?id=" + $('#CarsForCompany').val();
  };

  // Event binding for the dropdown change event
  $('#CarsForCompany').on('change', ShowCars);
  
  document.querySelectorAll('[name="status"]').forEach(button => {
    button.addEventListener('click', doFiltering);
  });

  timerJob();
  console.log('documentReady');
});

$(".uppercase").on('keyup', function () {
  let text = $(this).val();
  $(this).val(text.toUpperCase());
});

const clearErrors = () => {
  $(".validation-summary-errors").empty();
  var errorElements = document.getElementsByClassName('text-danger');
  for (var i = 0; i < errorElements.length; i++) {
    errorElements[i].innerHTML = '';
  }
};

const setupClearErrors = () => {
  clearErrors();
  var form = document.getElementById('CreateForm');
  if (form) {
    form.addEventListener('mousedown', clearErrors);
  }
}

document.addEventListener('DOMContentLoaded', setupClearErrors);

const timerJob = () => {
  const halfSecond = 500;
  const oneTenthSecond = 100;
  $.ajax({
    url: "http://localhost:63567/api/carapi/getallcars",
    type: "GET",
    dataType: "json",
    success: (cars) => {
      if (cars.length === 0) {
        setTimeout(timerJob, oneTenthSecond);
        console.log("Inga bilar hittade!");
        return;
      }
      const selectedItem = Math.floor(Math.random() * cars.length);
      let selectedCar = cars[selectedItem];
      if (selectedCar.disabled === true) {
        console.log(selectedCar.regNr + " is blocked for uppdating of Online/Offline!");
        return;
      }
      selectedCar.online = !selectedCar.online;
      $.ajax({
        url: 'http://localhost:63567/api/carapi/updateonline',
        contentType: "application/json",
        type: "POST",
        data: JSON.stringify(selectedCar),
        dataType: "json",
        success: (response) => {

        },
        error: (error) => {

        }
      });

      const selector = `#${selectedCar.id} td:eq(2)`;
      const selector2 = `#${selectedCar.id + "_2"} td:eq(3)`;
      const selector3 = `#${selectedCar.id + "_3"}`;
      if (selectedCar.online === true) {
        $(selector).text("Online");
        $(selector).removeClass("alert-danger");
        $(selector2).text("Online");
        $(selector2).removeClass("alert-danger");
        $(selector3).text("Online");
        $(selector3).removeClass("alert-danger");
        console.log(selectedCar.regNr + " is Online!");
      }
      else {
        $(selector).text("Offline");
        $(selector).addClass("alert-danger");
        $(selector2).text("Offline");
        $(selector2).addClass("alert-danger");
        $(selector3).text("Offline");
        $(selector3).addClass("alert-danger");
        console.log(selectedCar.regNr + " is Offline!");
      }
      if (document.getElementById("All") !== null) {
        doFiltering();
      }
    }
  });
  setTimeout(timerJob, halfSecond);
};

const doFiltering = () => {
  let selection = 0;
  let radiobtn = document.getElementById("All");
  if (radiobtn.checked === false) {
    radiobtn = document.getElementById("Online");
    if (radiobtn.checked === true) {
      selection = 1;
    }
    else {
      selection = 2;
    }
  }

  var table = $('#cars > tbody');
  $('tr', table).each(function () {
    $(this).removeClass("hidden");
    let td = $('td:eq(2)', $(this)).html();
    if (td !== undefined) {
      td = td.trim();
    }
    if (td === "Offline" && selection === 1) {
      $(this).addClass("hidden");  //Show only Online
    }
    if (td === "Online" && selection === 2) {
      $(this).addClass("hidden"); //Show only Offline
    }
  });
};
