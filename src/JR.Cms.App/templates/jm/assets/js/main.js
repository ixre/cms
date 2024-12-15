/***************************************************
==================== JS INDEX ======================
****************************************************
01. PreLoader Js
02. Mobile Menu Js
03. Mesti Menu Js
04. Sidebar Js
05. Search Js
06. Sticky Header Js
07. Data Background Js
08. Nice Select Js
09. Testimonial Slider Js
10. Testimonial Slider 2 Js
11. Brand Slider Js
12. MagnificPopup img view js
13. MagnificPopup video view js
14. Skill bar js
15. Circlechart js 
16. Tooltip js
17. Wow Js
18. Data width Js
19. Countdown Slider Js
20. Type Js
21. CodeMirror Editor js

****************************************************/

(function ($) {
	("use strict");

	var windowOn = $(window);
	////////////////////////////////////////////////////
	// 01. PreLoader Js
	windowOn.on("load", function () {
		$("#loading").fadeOut(500);
	});

	////////////////////////////////////////////////////
	// 02. Mobile Menu Js
	$("#mobile-menu").meanmenu({
		meanMenuContainer: ".mobile-menu",
		meanScreenWidth: "991",
		meanExpand: ['<i class="fal fa-plus"></i>'],
	});
	$("#mobile-menu2").meanmenu({
		meanMenuContainer: ".mobile-menu2",
		meanScreenWidth: "4000",
		meanExpand: ['<i class="fal fa-plus"></i>'],
	});
	////////////////////////////////////////////////////
	// 03. Mesti Menu Js
	$(function () {
		$('#toc').metisMenu();
	  });

	////////////////////////////////////////////////////
	// 04. Sidebar Js
	$(".side-info-close,.offcanvas-overlay").on("click", function () {
		$(".side-info").removeClass("info-open");
		$(".offcanvas-overlay").removeClass("overlay-open");
	});
	$(".side-toggle").on("click", function () {
		$(".side-info").addClass("info-open");
		$(".offcanvas-overlay").addClass("overlay-open");
	});


	////////////////////////////////////////////////////
	// 05. Search Js
	$(".search-toggle").on("click", function () {
		$(".search__area").addClass("opened");
	});
	$(".search-close-btn").on("click", function () {
		$(".search__area").removeClass("opened");
	});

	////////////////////////////////////////////////////
	// 06. Sticky Header Js
	windowOn.on("scroll", function () {
		var scroll = $(window).scrollTop();
		if (scroll < 100) {
			$(".du-sticky-header").removeClass("sticky");
		} else {
			$(".du-sticky-header").addClass("sticky");
		}
	});
	////////////////////////////////////////////////////
	// 07. Data Background Js
	$("[data-background").each(function () {
		$(this).css(
			"background-image",
			"url( " + $(this).attr("data-background") + "  )"
		);
	});

	////////////////////////////////////////////////////
	// 08. Nice Select Js
	$("select").niceSelect();

	///////////////////////////////////////////////////
	// 09. Testimonial Slider Js
	var swiper = new Swiper(".testimonial-nav", {
        spaceBetween: 15,
        slidesPerView: 3,
        freeMode: true,
		centeredSlides: true,
		loop: true,
        watchSlidesProgress: true,
		breakpoints: {
			768: {
				spaceBetween: 40,
			}
		}

      });
      var swiper2 = new Swiper(".du-testimonial", {
        spaceBetween: 10,
        navigation: {
          nextEl: ".swiper-button-next",
          prevEl: ".swiper-button-prev",
        },
        thumbs: {
          swiper: swiper,
        },
      });

	///////////////////////////////////////////////////
	// 10. Testimonial Slider 2 Js
	  if (jQuery(".testimonial-active-2").length > 0) {
		let brand = new Swiper(".testimonial-active-2", {
			slidesPerView: 2,
			spaceBetween: 100,
			// direction: 'vertical',
			loop: true,
			autoplay: {
				delay: 5000,
			},

			// If we need pagination
			pagination: {
				el: ".swiper-pagination-team",
				clickable: true,
			},

			// Navigation arrows
			navigation: {
				nextEl: ".swiper-button-next",
				prevEl: ".swiper-button-prev",
			},

			// And if we need scrollbar
			scrollbar: {
				el: ".swiper-scrollbar",
			},
			breakpoints: {
				0: {
					slidesPerView: 1,
				},
				550: {
					slidesPerView: 1,
				},
				768: {
					slidesPerView: 2,
					spaceBetween: 60,
				},
				1200: {
					slidesPerView: 2,
				},
			},
		});
		}

	///////////////////////////////////////////////////
	// 11. Brand Slider Js
	if (jQuery(".brand-active").length > 0) {
		let brand = new Swiper('.brand-active', {
			slidesPerView: 5,
			spaceBetween: 30,
			// direction: 'vertical',
			loop: true,
			autoplay: {
					delay: 5000,

				},
		  
			// If we need pagination
			pagination: {
			  el: '.swiper-pagination',
			  clickable: true,
			},
		  
			// Navigation arrows
			navigation: {
			  nextEl: '.swiper-button-next',
			  prevEl: '.swiper-button-prev',
			},
		  
			// And if we need scrollbar
			scrollbar: {
			  el: '.swiper-scrollbar',
			},
			breakpoints: {
				0: {
					slidesPerView: 2,
				  },
				550: {
				  slidesPerView: 3,
				},
				768: {
				  slidesPerView: 4,
				},
				1200: {
				  slidesPerView: 5,
				},
			  }
	
		  });
		}

	////////////////////////////////////////////////////
	// 12. MagnificPopup img view js

	$(".image-popups").magnificPopup({
		type: "image",
		gallery: {
			enabled: true,
		},
	});

	////////////////////////////////////////////////////
	// 13. MagnificPopup video view js
	$(".popup-video").magnificPopup({
		type: "iframe",
	});

	////////////////////////////////////////////////////
	// 14. Skill bar js

	var skillbar = $('.skillbar');
	if(skillbar.length) {
		$('.skillbar').skillBars({  
			from: 0,    
			speed: 3500,    
			interval: 100,  
			decimals: 0,    
		});
	}
	  
	////////////////////////////////////////////////////
	// 15. Circlechart js 

	$('.circlechart').circlechart(); 

	////////////////////////////////////////////////////
	// 16. Tooltip js

	var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
	var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
	  return new bootstrap.Tooltip(tooltipTriggerEl)
	})

	////////////////////////////////////////////////////
	// 17. Wow Js
	new WOW().init();

	////////////////////////////////////////////////////
	// 18. Data width Js
	$("[data-width]").each(function () {
		$(this).css("width", $(this).attr("data-width"));
	});

		///////////////////////////////////////////////////
	// 19. Countdown Slider Js
	$('#getting-started').countdown('2022/06/20', function(event) {
        $(this).html(event.strftime('<div class="default-countdown"><h2>%D</h2> <span>Days</span></div> <div class="default-countdown"><h2>%H</h2> <span>Hours</span></div> <div class="default-countdown"><h2>%M</h2> <span>Mins</span></div> <div class="default-countdown"><h2>%S</h2> <span>Secs</span></div>'));

      });
      $('#border-style').countdown('2022/06/20', function(event) {
        $(this).html(event.strftime('<div class="border-countdown"><h2>%D</h2> <span>Days</span></div> <div class="border-countdown"><h2>%H</h2> <span>Hours</span></div> <div class="border-countdown"><h2>%M</h2> <span>Mins</span></div> <div class="border-countdown"><h2>%S</h2> <span>Secs</span></div>'));

      });
      $('#multicolor-style').countdown('2022/06/20', function(event) {
        $(this).html(event.strftime('<div class="multicolor-countdown"><h2>%D</h2> <span>Days</span></div> <div class="multicolor-countdown"><h2>%H</h2> <span>Hours</span></div> <div class="multicolor-countdown"><h2>%M</h2> <span>Mins</span></div> <div class="multicolor-countdown"><h2>%S</h2> <span>Secs</span></div>'));

      });


    ////////////////////////////////////////////////////
	// 20. Type Js

	if ($(".animate-type").length > 0) {
		var typed = new Typed('.animate-type', {
			strings: ["Developer."],
			startDelay: 60,
			backSpeed: 60,
			loop: true,
		});
	}

	////////////////////////////////////////////////////
	// 21. CodeMirror code editor Js
	if ($("#editorBlackboard").length > 0) {
		var editor = CodeMirror.fromTextArea
		(document.getElementById('editorBlackboard'), {
		mode: "xml",
		theme: "blackboard",
		lineNumbers: true,
		autoCloseTags: true
		})
	}

	if ($("#editorcompiler").length > 0) {
	var editor = CodeMirror.fromTextArea
	(document.getElementById('editorcompiler'), {
	  mode: "xml",
	  theme: "dracula",
	  lineNumbers: true,
	  autoCloseTags: true
	})
   }
   if ($("#editorcobalt").length > 0) {
   var editor = CodeMirror.fromTextArea
   (document.getElementById('editorcobalt'), {
	 mode: "xml",
	 theme: "cobalt",
	 lineNumbers: true,
	 autoCloseTags: true
   })
   }

   if ($("#editor-base16").length > 0) {
   var editor = CodeMirror.fromTextArea
   (document.getElementById('editor-base16'), {
	 mode: "xml",
	 theme: "xq-light",
	 lineNumbers: true,
	 autoCloseTags: true
   })
   }
   
   if ($("#editor-duotone-light").length > 0) {
   var editor = CodeMirror.fromTextArea
   (document.getElementById('editor-duotone-light'), {
	 mode: "xml",
	 theme: "duotone-light",
	 lineNumbers: true,
	 autoCloseTags: true
   })
   }

})(jQuery);