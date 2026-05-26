function setCookie(key, value) {
  var expires = new Date();
  expires.setTime(expires.getTime() + (365 * 24 * 60 * 60 * 1000));
  document.cookie = key + '=' + value + ';expires=' + expires.toUTCString() + ';path=/';
}

function getCookie(key) {
  var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
  return keyValue ? keyValue[2] : null;
}

(function () {
  function initSwitcher() {
    if (typeof jQuery === 'undefined' || typeof window.jQuery === 'undefined') {
      setTimeout(initSwitcher, 100);
      return;
    }

    var $ = window.jQuery || jQuery;

    $(document).ready(function () {
      var savedTheme = getCookie('theme');
      if (savedTheme && savedTheme != 'default') {
        if (!$('#theme-stylesheet').length) {
          $('head').append('<link href="/algocak/css/themes/' + savedTheme + '.css" rel="stylesheet" id="theme-stylesheet">');
        }
      }

      $(document).on("click", ".switcher-btn", function (event) {
        event.preventDefault();
        event.stopPropagation();
        var $switcher = $("#switcher-options");
        var currentLeft = $switcher.css('left');
        if (currentLeft == '0px' || currentLeft == '0') {
          $switcher.animate({ left: '-176px' }, 500);
        } else {
          $switcher.animate({ left: 0 }, 500);
        }
        return false;
      });

      $(document).on('click', ".theme-colour", function (event) {
        event.preventDefault();
        event.stopPropagation();

        var theme = $(this).attr('data-name');
        if (!theme) return false;

        setCookie('theme', theme);

        $("#theme-stylesheet").remove();

        if (theme != 'default') {
          $('head').append('<link href="/algocak/css/themes/' + theme + '.css" rel="stylesheet" id="theme-stylesheet">');
        }

        return false;
      });
    });
  }

  if (document.readyState === 'complete' || document.readyState === 'interactive') {
    initSwitcher();
  } else {
    window.addEventListener('DOMContentLoaded', initSwitcher);
    window.addEventListener('load', initSwitcher);
  }
})();