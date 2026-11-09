history.scrollRestoration = "manual";

$(window).on('beforeunload', function(){
      $(window).scrollTop(0);
});


//header add class
$(window).scroll(function () {
  if ($(this).scrollTop() > 50) {
    $(".fixed-top").addClass("sticked");
  } else {
    $(".fixed-top").removeClass("sticked");
  }
});

//on click move to browser top
$(document).ready(function () {
  $(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
      $("#movetop").fadeIn();
    } else {
      $("#movetop").fadeOut();
    }
  });
  //click event to scroll to top
  $("#movetop").click(function () {
    $("html, body").animate({ scrollTop: 0 }, 200);
  });
});

document.onreadystatechange = function () {
  var state = document.readyState
  if (state == 'complete') {
      setTimeout(function () {
          document.getElementById('load').style.visibility = "hidden";
      }, 1000);
  }
}

//home slider 
var swiper = new Swiper('.homeswiper-container', {
  spaceBetween: 30,
  effect: 'fade',
  autoplay: {
    delay: 5000,
    disableOnInteraction: true,
  },
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
  } 
});

//home page services swiper
var swiper = new Swiper('.service-swiper', {
  // slidesPerView: 4,
  spaceBetween: 10,
 autoplay: {
    delay: 5000,
    disableOnInteraction: true,
  },
  // init: false,
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
  },
  breakpoints: {
    640: {
      slidesPerView: 2,
      spaceBetween: 0,
    },
    768: {
      slidesPerView: 2,
      spaceBetween: 0,
    },
    1024: {
      slidesPerView: 4,
      spaceBetween: 10,
    },
  }
});



document.onreadystatechange = function () {
  var state = document.readyState
  if (state == 'complete') {
      setTimeout(function () {
          document.getElementById('load').style.visibility = "hidden";
      }, 1000);
  }
}


history.scrollRestoration = "manual";

$(window).on('beforeunload', function(){
      $(window).scrollTop(0);
});




//home page services 
var swiper = new Swiper('.home-testimonials', {
  slidesPerView: 1,
  spaceBetween: 10,
 autoplay: {
    delay: 5000,
    disableOnInteraction: true,
  },
  // init: false,
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
  },
  breakpoints: {
    640: {
      slidesPerView: 2,
      spaceBetween: 20,
    },
    768: {
      slidesPerView: 2,
      spaceBetween: 40,
    },
    1024: {
      slidesPerView: 2,
      spaceBetween: 10,
    },
    1400: {
      slidesPerView: 2,
      spaceBetween: 10,
    },
  }
});

//retailers swiper
var swiper = new Swiper('.retailers-swiper', {
  slidesPerView: 2,
  spaceBetween: 20,
  autoplay: {
    delay: 4000,
    disableOnInteraction: true,
  },
  navigation: {
    nextEl: '.swiper-button-next',
    prevEl: '.swiper-button-prev',
  },
  breakpoints: {
    480: {
      slidesPerView: 2,
      spaceBetween: 15,
    },
    640: {
      slidesPerView: 3,
      spaceBetween: 20,
    },
    768: {
      slidesPerView: 4,
      spaceBetween: 25,
    },
    1024: {
      slidesPerView: 5,
      spaceBetween: 30,
    },
    1400: {
      slidesPerView: 6,
      spaceBetween: 30,
    },
  }
});


