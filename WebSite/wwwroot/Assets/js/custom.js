
$(document).ready(function () {
    $('.image-grid .col-12').each(function (index) {
        var img = $(this).find('img');
        var currentAlt = img.attr('alt') || '';
        img.attr('alt', currentAlt + ' - Image ' + (index + 1));
    });
});
document.querySelectorAll('.col-6').forEach(el => {
    el.classList.remove('col-6');
    el.classList.add('col-lg-6');
});
document.querySelectorAll('.col-4').forEach(el => {
    el.classList.remove('col-4');
    el.classList.add('col-lg-4');
});
document.addEventListener('DOMContentLoaded', function () {
    const strip = document.querySelector('.strip');
    const hero = document.querySelector('.hero');
    if (strip && hero) {
        const parts = strip.textContent.split('>').map(part => part.trim());
        const nav = document.createElement('nav');
        nav.classList.add('customNav');
        nav.setAttribute('aria-label', 'breadcrumb');
        const ol = document.createElement('ol');
        ol.classList.add('breadcrumb');
        let path = '/English';
        parts.forEach((part, index) => {
            const li = document.createElement('li');
            li.classList.add('breadcrumb-item');
            if (index === 0) {
                const a = document.createElement('a');
                a.href = '/English/Index';
                a.textContent = 'Home';
                li.appendChild(a);
            } else if (index === parts.length - 1) {
                li.classList.add('active');
                li.setAttribute('aria-current', 'page');
                li.textContent = part;
            } else {
                const slug = part.replace(/\s+/g, '-');
                path += `/${slug}`;
                const a = document.createElement('a');
                a.href = path;
                a.textContent = part;
                li.appendChild(a);
            }
            ol.appendChild(li);
        });
        nav.appendChild(ol);
        strip.remove();
        hero.insertAdjacentElement('afterend', nav);
    }
    setTabIndexAsync()
});
document.addEventListener('DOMContentLoaded', function () {
    $('.has-sub').each(function () {
        const $menuItem = $(this);
        const $link = $menuItem.find('> a');
        const $submenu = $menuItem.find('> ul');
        // $link.attr({
        //     'aria-haspopup': 'false',
        //     'aria-expanded': 'false',

        // });
        $link.on('click keydown', function (e) {
            if (e.type === 'click' || e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                const expanded = $link.attr('aria-expanded') === 'true';
                $link.attr('aria-expanded', !expanded);
                $link.attr($.trim($link.text()) + (expanded ? '' : ''));
                $submenu.toggle();
            }
        });
    });
});
// ============15-05-2025===============
$(document).ready(function () {
    $('img:not([alt])').each(function () {
        $(this).attr('alt', '');
    });
    document.querySelectorAll('.hero .item img').forEach(img => {
        img.alt = '';
    });
});
// ============ End ===============

$("#skip").click(function () {
    const heroHeight = $(".hero").outerHeight();
    if (heroHeight) {
        $('html, body').animate({
            scrollTop: heroHeight - 60
        }, 'slow');
    }
});

var owl = $(".hero-carousal");
owl.owlCarousel({
    smartSpeed: 1000,
    items: 1,
    loop: true,
    margin: 0,
    nav: true,
    dots: true,
    autoplay: true,
    autoplayTimeout: 5000,
    onInitialized: function (event) {
        // Set custom titles
        $(".owl-prev").attr("title", "Previous");
        $(".owl-next").attr("title", "Next");
    }
});
$(".owlCtrl-1 .play").on("click", function () {
    $(".hero-carousal").trigger("play.owl.autoplay", [5000]);
    $(this).addClass("active");
    $(".owlCtrl-1 .stop").removeClass("active");
    setOwlCarouselAriaAttributes()
});

// Stop Button - stops autoplay only for .hero-carousal
$(".owlCtrl-1 .stop").on("click", function () {
    $(".hero-carousal").trigger("stop.owl.autoplay");
    $(this).addClass("active");
    $(".owlCtrl-1 .play").removeClass("active");
    setOwlCarouselAriaAttributes()
});


//$('.text-end').append(' Designed & Developed by <a style="text-decoration:underline" href="https://www.planetecomsolutions.com/"  target="_blank">PECS</a>');

$('.logo a').attr("href", "/");

$('.play').on('click', function () {
    owl.trigger('play.owl.autoplay', [5000]);
    setOwlCarouselAriaAttributes()
})
$('.stop').on('click', function () {
    owl.trigger('stop.owl.autoplay');
    setOwlCarouselAriaAttributes()
})
$(".owlCtrl-1 span").click(function () {
    $(".owlCtrl-1 span").removeClass("active");
    $(this).addClass("active");
});


var owl2 = $(".vessels-carousal");
owl2.owlCarousel({
    smartSpeed: 1000,
    items: 7,
    loop: true,
    autoplay: true,
    autoplayTimeout: 5000,
    margin: 10,
    nav: true,
    dots: false,
    onInitialized: function (event) {
        // Set custom titles
        $(".owl-prev").attr("title", "Previous");
        $(".owl-next").attr("title", "Next");
    },
    responsive: {
        0: { items: 1, stagePadding: 40 },
        500: { items: 3 },
        769: { items: 5 },
        1200: { items: 7 }
    },
});
$('.play2').on('click', function () {
    owl2.trigger('play.owl.autoplay', [5000]);
    setOwlCarouselAriaAttributes();
})
$('.stop2').on('click', function () {
    owl2.trigger('stop.owl.autoplay')
})
$(".owlCtrl-2 span").click(function () {
    $(".owlCtrl-2 span").removeClass("active");
    $(this).addClass("active");
    setOwlCarouselAriaAttributes();
});



$('.item').hover(function (e) {
    var anchor = $(this).attr('data-id');
    $('.' + anchor).toggleClass('active');
})


var owl3 = $(".gallery-carousal");
owl3.owlCarousel({
    smartSpeed: 1000,
    items: 6,
    margin: 16,
    nav: true,
    dots: false,
    center: true,
    loop: true,
    autoplay: true,
    autoplayTimeout: 5000,
    mouseDrag: false,
    touchDrag: false,
    responsive: {
        0: { items: 1, stagePadding: 35 },
        500: { items: 3 },
        789: { items: 3 },
        1201: { items: 3 }
    },
    onInitialized: function (event) {
        // Set custom titles
        $(".owl-prev").attr("title", "Previous");
        $(".owl-next").attr("title", "Next ");
    }
});
$('.play3').on('click', function () {
    owl3.trigger('play.owl.autoplay', [5000])
    setOwlCarouselAriaAttributes()
})
$('.stop3').on('click', function () {
    owl3.trigger('stop.owl.autoplay')
})
$(".owlCtrl-3 span").click(function () {
    $(".owlCtrl-3 span").removeClass("active");
    $(this).addClass("active");
    setOwlCarouselAriaAttributes()
});


var owl4 = $(".media-carousal");
owl4.owlCarousel({
    smartSpeed: 1000,
    items: 1,
    loop: true,
    margin: 0,
    nav: false,
    dots: true,
    autoplay: true,
    autoplayTimeout: 5000,
    onInitialized: function (event) {
        // Set custom titles
        $(".owl-prev").attr("title", "Previous");
        $(".owl-next").attr("title", "Next ");
    }
});
$(".owlCtrl-4 span").click(function () {
    $(".owlCtrl-4 span").removeClass("active");
    $(this).addClass("active");
});
$('.play4').on('click', function () {
    owl4.trigger('play.owl.autoplay', [5000])
    setOwlCarouselAriaAttributes();
})
$('.stop4').on('click', function () {
    owl4.trigger('stop.owl.autoplay');
    setOwlCarouselAriaAttributes()
})


var owl5 = $(".logo-carousal");
owl5.owlCarousel({
    smartSpeed: 1000,
    items: 6,
    margin: 16,
    nav: true,
    dots: false,
    autoplay: true,
    autoplayTimeout: 5000,
    loop: true,
    responsive: {
        0: { items: 1, stagePadding: 50 },
        500: { items: 5 },
        1199: { items: 7 },
    },
    onInitialized: function (event) {
        // Set custom titles
        $(".owl-prev").attr("title", "Previous");
        $(".owl-next").attr("title", "Next ");
    }
});
$(".owlCtrl-5 span").click(function () {
    $(".owlCtrl-5 span").removeClass("active");
    $(this).addClass("active");
});
$('.play5').on('click', function () {
    owl5.trigger('play.owl.autoplay', [5000]);
    setOwlCarouselAriaAttributes()
})
$('.stop5').on('click', function () {
    owl5.trigger('stop.owl.autoplay')
    setOwlCarouselAriaAttributes()
})


$(document).ready(function () {
    function getIndianDateTime() {
        const now = new Date();
        const istOffset = 5.5 * 60;
        const localOffset = now.getTimezoneOffset();
        const istTime = new Date(now.getTime() + (istOffset + localOffset) * 60000);
        const day = String(istTime.getDate()).padStart(2, '0');
        const month = String(istTime.getMonth() + 1).padStart(2, '0');
        const year = istTime.getFullYear();
        return { istTime, formattedDate: `${day}-${month}-${year}` };
    }

    const randomTimes = [
        "9:45 AM", "10:30 AM", "11:15 AM", "12:05 PM", "1:50 PM",
        "2:35 PM", "3:25 PM", "4:40 PM", "5:10 PM", "6:25 PM"
    ];

    function getDailyStaticTime() {
        const storageKey = "dailyStaticTime";
        const indexKey = "dailyStaticTimeIndex";
        const { istTime, formattedDate } = getIndianDateTime();
        const saved = JSON.parse(localStorage.getItem(storageKey));
        const savedIndex = parseInt(localStorage.getItem(indexKey)) || 0;
        const cutoff = new Date(istTime);
        cutoff.setHours(18, 30, 0, 0);
        let shouldUpdate = false;
        if (!saved || saved.date !== formattedDate) {
            if (istTime >= cutoff) {
                shouldUpdate = true;
            } else if (!saved) {
                shouldUpdate = true;
            }
        }
        if (shouldUpdate) {
            const nextIndex = (savedIndex + 1) % randomTimes.length;
            const newEntry = {
                date: formattedDate,
                time: randomTimes[nextIndex]
            };
            localStorage.setItem(storageKey, JSON.stringify(newEntry));
            localStorage.setItem(indexKey, nextIndex);
            return `${formattedDate} ${newEntry.time}`;
        }
        return `${formattedDate} ${saved.time}`;
    }
    const finalIndianDateTime = getDailyStaticTime();

    $(".dataTable").removeClass("table-striped");

    $('.news').easyTicker({
        visible: 2,
        controls: {
            toggle: '.bttest',
        },
    });
    $(".bttest").click(function () {
        $(".bttest").removeClass("active");
        $(this).addClass("active");

    });

    let updateDate = $(".footer-single > h5").html();
    let lastUpdatedDiv = document.querySelector(".last-updated");
    if (lastUpdatedDiv) {
        lastUpdatedDiv.innerHTML = `${updateDate}`;
    }

    let footerUpdatedDiv = document.querySelector(".footer-update");
    let footerDummy = finalIndianDateTime;
    if (footerUpdatedDiv) {
        footerUpdatedDiv.innerHTML = `<b>${updateDate.split(" ")[0]} ${updateDate.split(" ")[1]} ${updateDate.split(" ")[2]}: </b>${footerDummy}`;
    }

    $('.news2').easyTicker({
        visible: 3,
        controls: {
            toggle: '.bttest2',
        },
    });
    $(".bttest2").click(function () {
        $(".bttest2").removeClass("active");
        $(this).addClass("active");
    });


    $('.logo-sec .owl-item .item img').each(function (i, obj) {
        var string = $(this).attr("src");
        $(this).attr("src", string.slice(1));
    });

});


$('.image-link').magnificPopup({
    type: 'image',
    closeOnContentClick: true,
    //closeBtnInside: false,
    fixedContentPos: true,
    mainClass: 'mfp-no-margins mfp-with-zoom', // class to remove default margin from left and right side
    image: {
        verticalFit: true
    },
    zoom: {
        enabled: true,
        duration: 300 // don't foget to change the duration also in CSS
    }
});

// -----------------------------------------------------------------




$(window).scroll(function () {
    var sticky = $('.nav-holder'),
        scroll = $(window).scrollTop();
    if (scroll >= 100) sticky.addClass('fixed');
    else sticky.removeClass('fixed');
});


$(".site-nav ul li a").each(function () {
    if ($(this).parent().find('ul').length > 0) {
        $(this).parent().prepend('<span class="subDropAlt"></span>');
        $(this).parent().addClass('has-sub');
    } else {
    }
});

$('.subDropAlt').click(function () {
    $(this).parent().find('> ul').slideToggle();
});


$('.mobClick1').click(function () {
    $(this).toggleClass('open');
    $('.site-nav').toggleClass('act');
});

$('.mobClick2').click(function () {
    $(this).toggleClass('open');
    $('.header-top-right').toggleClass('act');
});

$('.table.table-striped').addClass('table-responsive');


// 6 august 2024
$(".mid-content table").addClass('table-responsive');
// 29 august 2024 
$('.table.table-bordered').parent().addClass('table-responsive');
// scroll to top
const scrollButton = $('<a href="#" aria-label="scroll to top" class="scrollup aCt" ><strong class="fa fa-angle-up"> </strong></a>');
$('body').append(scrollButton);

$(window).scroll(function () {
    if ($(this).scrollTop() > 200) {
        $('.scrollup').fadeIn().addClass('aCt');
    } else {
        $('.scrollup').fadeOut().removeClass('aCt');
    }
});

$('.scrollup').click(function () {
    $("html, body").animate({ scrollTop: 0 }, 600);
    return false;
});

const backButton = $('<button class="btn btn-primary backBtn"> Back to previous Page</button> ');
$('.mid-content > .container:first').append(backButton);

$('.backBtn').click(function () {
    window.history.back();
});


const elements = document.querySelectorAll('.footer-bottom-main .row > div , .footer-update , .site-nav > ul > li.has-sub,  #skip , .owlCtrl > span  ,.ctrlBtn > span , #text-width , .footer-single p , .facili .img , form form-label , .leadership td , .inves-img b ,  .inves-img b ,  .inves-img .content ul li');
elements.forEach(function (element, index) {
    element.setAttribute('tabindex', 0);
});


var pageBody = document.body;
var toggleButton = document.getElementById("text-width");
var toggleClass = "textWidth";
if (localStorage.getItem("textWidthEnabled") === "true") {
    pageBody.classList.add(toggleClass);
}
toggleButton.addEventListener("click", function () {
    pageBody.classList.toggle(toggleClass);
    localStorage.setItem("textWidthEnabled", pageBody.classList.contains(toggleClass));
});


$(".vessels , .mid-content > .container ").attr("id", "skipCont");

$(".site-nav ul a").each(function () {
    $(this).attr("title", $(this).text());
});

// const lang = document.documentElement.lang; 
//         const prevButton = document.querySelector('.carousel-control-prev');
//         const nextButton = document.querySelector('.carousel-control-next');

//         if (lang === 'hi') {
//             prevButton.setAttribute('title', 'पिछला स्लाइड');
//             nextButton.setAttribute('title', 'अगला स्लाइड');
//         } else {
//             prevButton.setAttribute('title', 'Previous Slide');
//             nextButton.setAttribute('title', 'Next Slide');
//         }

//         new bootstrap.Tooltip(prevButton);
//         new bootstrap.Tooltip(nextButton);

// function updateNavigation(disableAll = false) {
//   $(".owl-item a").attr("tabindex", "-1");  
//   if (!disableAll) {
//     $(".owl-item.active a").each(function () {
//       $(this).attr("tabindex", "0");  
//     });
//   }
// }

$(document).ready(function () {
    var $carousel = $(".owl-carousel");

    if ($carousel.length && typeof updateNavigation === "function") {
        $carousel
            .on("translate.owl.carousel", function () {
                updateNavigation(true);
            })
            .on("translated.owl.carousel", function () {
                updateNavigation(false);
            });
    }
});

document.querySelectorAll(".row:has(figure) > div, .popUpimg , .backBtn").forEach(div => {
    div.setAttribute("tabindex", '0');
})

document.querySelectorAll('.row:has(.guestbook-table) , .backBtn').forEach((container) => {
    const focusables = Array.from(
        container.querySelectorAll('input:not([type="hidden"]):not([disabled]), textarea:not([disabled]), button:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="0"]) , a , .backBtn')
    );
    container.addEventListener('keydown', (e) => {
        const active = document.activeElement;
        const currentIndex = focusables.indexOf(active);
        if (currentIndex === -1) return;
        if (e.key === 'Tab') {
            e.preventDefault();
            const dir = e.shiftKey ? -1 : 1;
            let nextIndex = (currentIndex + dir + focusables.length) % focusables.length;
            while (focusables[nextIndex].offsetParent === null || focusables[nextIndex].disabled) {
                nextIndex = (nextIndex + dir + focusables.length) % focusables.length;
            }
            focusables[nextIndex].focus();
            return;
        }
        if (active.type === 'radio' && ['ArrowRight', 'ArrowDown', 'ArrowLeft', 'ArrowUp'].includes(e.key)) {
            e.preventDefault();
            const name = active.name;
            const radios = focusables.filter(el => el.type === 'radio' && el.name === name);
            const i = radios.indexOf(active);
            if (i === -1) return;
            let nextRadio;
            if (['ArrowRight', 'ArrowDown'].includes(e.key)) {
                nextRadio = radios[(i + 1) % radios.length];
            } else {
                nextRadio = radios[(i - 1 + radios.length) % radios.length];
            }
            nextRadio.focus();
        }
    });
});


const div = document.querySelector('div.heading');
if (div) {
    const h1 = document.createElement('h1');
    h1.className = div.className;
    h1.innerHTML = div.innerHTML;
    div.parentNode.replaceChild(h1, div);
}
const currentLang = document.documentElement.lang; // 'en' or 'hi'
const oppositeLang = currentLang === 'en' ? 'hi' : 'en';

$('#language').attr({ lang: oppositeLang, hreflang: oppositeLang });
// Don't change the text inside the element

//   focus elecments should be dimiss when click on esc button 
document.addEventListener('keydown', function (e) {
    if (e.key === "Escape") {
        if (document.activeElement && typeof document.activeElement.blur === 'function') {
            document.activeElement.blur();
        }
    }
});
//owl carousel screen reader 
document.querySelectorAll('.owl-carousel').forEach(carousel => {
    carousel.setAttribute('role', 'region');
    carousel.setAttribute('aria-label', 'Carousel');
});
document.querySelectorAll('.owl-stage-outer').forEach(stageOuter => {
    stageOuter.setAttribute('role', 'status');
});
$("hr").attr("role", "presentation");
function headingChange() {
    const h4 = document.querySelector('.inner-heading');
    if (!h4) return; // Exit silently if the element is not found

    const h2 = document.createElement('h2');
    h2.className = h4.className;
    h2.style.cssText = h4.style.cssText;
    h2.innerHTML = h4.innerHTML;
    h4.parentNode.replaceChild(h2, h4);
}

headingChange();

const rows = document.querySelectorAll('table.table.mt-2.ml-4.table-bordered.contact-table.boldtable.table-responsive tr');

rows.forEach(row => {
    const tds = row.querySelectorAll('td');
    if (tds.length >= 2) {
        const text = tds[0].innerText.trim();
        const pdfLink = tds[1].querySelector('a');
        if (pdfLink) {
            pdfLink.setAttribute('aria-label', text);
        }
    }
});
const td = document.querySelector('table.table.mt-2.ml-4.table-bordered.contact-table.boldtable.table-responsive tr td:nth-child(3)');
if (td) {
    const anchor = td.querySelector('a');
    if (anchor?.href) {
        const fileName = anchor.href.split('/').pop();
        const cleanedFileName = fileName.replace(/-/g, '');
        anchor.setAttribute('aria-label', cleanedFileName);
    }
}
function updateAriaSelected($select) {
    $select.find('option').each(function () {
        $(this).attr('aria-selected', this.selected ? 'true' : 'false');
    });
}
$(function () {
    const $vendorType = $('#vendorType');
    $vendorType.on('mousedown focus change', function () {
        updateAriaSelected($vendorType);
    });
});
const btn = document.getElementById("captchaRefresh");
if (btn) {
    // Make sure it's focusable
    btn.setAttribute("tabindex", "0");
    btn.setAttribute("role", "button");
    // Handle Enter or Space keys to simulate click
    btn.addEventListener("keydown", function (e) {
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            btn.click();
        }
    });
}
//if (className != "") {
//    $('html,body').animate({
//        scrollTop: $(className).offset().top
//    }, 'slow');
//} 
function speakCaptcha() {
    fetch("../Home/GetCaptchaCode")
        .then(response => response.json())
        .then(data => {
            const captcha = data.captcha;

            if (!captcha || captcha.length === 0) {
                alert("Captcha not loaded");
                return;
            }

            const msg = new SpeechSynthesisUtterance(captcha);
            msg.lang = 'en-US';
            msg.rate = 0.5;
            window.speechSynthesis.speak(msg);
        })
        .catch(err => {
            console.error("Failed to get captcha", err);
            alert("Failed to get captcha.");
        });
}

// 16-06-2025
//async function setTabIndexAsync() {
//    $('.breadcrumb a').each(function () {
//        $(this).attr('aria-label', $(this).text().trim());
//    });
//    $('.breadcrumb-item.active').each(function () {
//        $(this).attr('aria-label', $(this).text().trim());
//    });
//    $('.breadcrumb-item.active').attr('tabindex', '0');
//}

$('.tab-accordian , .row > .col-sm-6:has(.finance-item) , .light-box , .rgt').attr('tabindex', '0');

$(".text-opt a , .color-opt button").attr("aria-pressed", "false");
$(".text-opt a , .color-opt button").on("click", function () {
    $(".text-opt a , .color-opt button").attr("aria-pressed", "false");
    $(this).attr("aria-pressed", "true");
});

document.querySelectorAll('.site-nav > ul > li.has-sub , .leadership .btns button ').forEach(el => {
    el.setAttribute('tabindex', '-1');
});
$('.search').attr({
    'role': 'search',
    'aria-label': 'Site Search'
});

$('#output').attr({
    'role': 'searchbox',
    'aria-label': 'Search input',
    'id': 'search-input'
});

$('.text-opt a').each(function () {
    var title = $(this).attr('title');
    if (title) {
        $(this).attr('aria-label', title);
        $(this).attr('role', 'button');
    }
});
$('.color-opt button').each(function () {
    var title = $(this).attr('title');
    if (title) {
        $(this).attr('aria-label', title);
    }
});

// Set initial state
$('.input-group .input-group-append .input-group-text ,   .input-group-append ').attr('tabindex', '-1');
$('#captchaRefresh , .input-group button').attr('tabindex', '0');
// Override aria-expanded after the original click handler
jQuery('.titleWrapper').each(function () {
    $(this).attr('aria-expanded', 'false');
});
$(document).on('click', '.titleWrapper', function () {
    $(this).attr('aria-expanded', $(this).hasClass('active') ? 'true' : 'false');
});

$(document).on("keydown", "input[type=checkbox], input[type=radio]", function (e) {
    if (e.key === "Enter" || e.which === 13) {
        if (this.type === "checkbox") {
            this.checked = !this.checked;
        } else if (this.type === "radio") {
            this.checked = true;
        }

        // Stop the other Enter handler
        e.preventDefault();
        e.stopImmediatePropagation();
    }
});


$('#tbl-career tr').each(function () {
    var thirdText = $(this).find('td:nth-child(3)').text().trim();
    $(this).find('td:nth-child(4)').attr('aria-label', thirdText);
});
//10-07-2025 

//$(function () {
//    $('div.head ul').each(function () {
//        const $ul = $(this);
//        const $div = $('<div></div>').attr('class', $ul.attr('class'));
//        $div.append($ul.contents());
//        $ul.replaceWith($div);
//        $div.find('li').eq(1).attr('aria-hidden', 'true');
//    });
//});

$(".tab__btns .tab__btn:has(a) , .interest .open-button").attr("tabindex", "-1");

$(function () {
    const $chart = $('#chart-element');
    $chart.removeAttr('tabindex');
    $chart.removeAttr('role');
});

$('button.tab__btn').attr('aria-expanded', 'false');
$('button.tab__btn.tab__btn--active').attr('aria-expanded', 'true');

$('.owlCtrl span:has(i[title="Play"]) , .ctrlBtn span:has(i[title="Play"]) ').attr({
    'role': 'button',
    'aria-label': 'Play carousel',
    'aria-hidden': 'false',
    'aria-pressed': 'false'
});

$('.owlCtrl span:has(i[title="Pause"]) , .ctrlBtn span:has(i[title="Pause"]) ').attr({
    'role': 'button',
    'aria-label': 'Pause carousel',
    'aria-hidden': 'false',
    'aria-pressed': 'false'
});
$(".ctrlBtn span.active , .owlCtrl span.active").attr('aria-pressed', 'true');

function setOwlCarouselAriaAttributes() {
    $('.owlCtrl span').each(function () {
        if ($(this).hasClass('active')) {
            $(this).attr('aria-pressed', 'true');
        } else {
            $(this).attr('aria-pressed', 'false');
        }
    });
}
$('.ctrlBtn span').click(function () {
    $('.ctrlBtn span').attr('aria-pressed', 'false');
    $(this).attr('aria-pressed', 'true');
});

document.querySelectorAll('.tree a').forEach(function (el) {
    el.setAttribute('aria-hidden', 'true');
});
document.querySelectorAll('.tab__btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.tab__btn').forEach(b => {
            b.setAttribute('aria-expanded', 'false');
        });
        btn.setAttribute('aria-expanded', 'true');
    });
});

$('.date h6, .mnt h6').each(function () {
    var content = $(this).html();
    $(this).html('<span>' + content + '</span>');
});
//document.querySelectorAll('li.breadcrumb-item > a').forEach(link => {
//    if (link.textContent.trim().toLowerCase() === 'home') {
//        link.setAttribute('href', '/');
//    }
//});
//document.querySelectorAll(".customNav a").forEach(link => {
//    const url = link.href;
//    if (url.startsWith(location.origin)) {
//        fetch(url, { method: "HEAD" })
//            .then(response => {
//                if (!response.ok) {
//                    link.style.pointerEvents = "none";
//                    link.style.opacity = "0.5";
//                    link.title = "Page not found";
//                }
//            })
//            .catch(() => {
//                link.style.pointerEvents = "none";
//                link.style.opacity = "0.5";
//                link.title = "Page not available";
//            });
//    }
//});

//for annouce items of press release 
$(".desWrapper .row").attr("role", "list");
$(".desWrapper .row .col-lg-6").attr("role", "listitem");
$("#search-input").attr('autocomplete', 'on')
//hero slide image dot indexing 
document.querySelectorAll('.hero-carousal .owl-dots .owl-dot').forEach((dot, index) => {
    dot.setAttribute('aria-label', `Banner image ${index + 1}`);
});
//hero slide image dot indexing 
document.querySelectorAll('.media-carousal .owl-dots .owl-dot').forEach((dot, index) => {
    dot.setAttribute('aria-label', `Media item ${index + 1}`);
});
//career archive data indexing 
$(".dataTable td:not(:has(a)):not(:has(button))").attr("tabindex", "0");


async function applyTabindexToDataTable() {
    try {
        $(".dataTable td:not(:has(a)):not(:has(button))").attr("tabindex", "0");
        console.log("Tabindex applied successfully.");
    } catch (error) {
        console.error("Error applying tabindex:", error);
    }
}

$('.desWrapper .row[role="list"]').each(function () {
    $(this).find('[role="listitem"]').each(function (index) {
        var listIndex = index + 1;
        var linkText = $(this).find('.head a').text().trim();
        $(this).find('.head a').attr('aria-label', 'List ' + listIndex + ': ' + linkText);
    });
});

$(document).ready(function () {
    $('a[aria-haspopup]').removeAttr('aria-haspopup');
    $(".media-img img").removeAttr("title"); 

    document.querySelectorAll("[fdprocessedid]").forEach(el => {
        el.removeAttribute("fdprocessedid");
    });
    $(".newsList a:empty").remove();

    $("i.fa-play-circle-o, i.fa-pause-circle-o").each(function () {
        var title = $(this).attr("role", "button");
    });

    $(".mid-content li").each(function () {
        let parent = $(this).parent();
        if (!parent.is("ul, ol")) {
            $(this).siblings("li").addBack().wrapAll("<ul class='addedUl'></ul>");
        }
    });

});
$("button").removeAttr("role");
const popup = document.getElementById("popup");
const img = popup.querySelector("img");
const images = [
    "assets/images/HarGharTirangaLogo.jpg",
    "assets/images/Online Supplier Meet 17.10.25.jpg"
];
let index = 0;
setInterval(() => {
    index = (index + 1) % images.length;
    img.src = images[index];
}, 7000);


document.getElementById("search-input").addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("datasubmit").click();
    }
});