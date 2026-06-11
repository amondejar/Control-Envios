var URL = BaseURL + /Home/;
$(document).ready(function () {
    // fade in .navbar

    $(window).scroll(function () {
        // set distance user needs to scroll before we start fadeIn
        if ($(this).scrollTop() > 100) {
            $('#top').css('background-color', '#000000');
        } else {
            $('#top').css('background-color', 'transparent');
        }
    });
  
});
