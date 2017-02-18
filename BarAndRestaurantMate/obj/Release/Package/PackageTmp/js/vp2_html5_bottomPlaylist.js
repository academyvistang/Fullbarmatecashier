/*
 * HTML5 Video Player with Bottom Playlist v4.5
 *
 * Copyright 2012-2015, LambertGroup
 * 
*/

(function ($) {
    $.fn.vp2_html5_bottomPlaylist_Video = function (options) {
        var videoID;
        //fullscreen vars
        var bodyOrigMargin;
        var bodyOrigOverflow;
        //border
        var videoBorderOrigPosition;
        var videoBorderOrigZIndex;
        //video
        var videoIsFullScreen = false;
        var videoOrigPosition;
        var videoOrigLeft;
        var videoOrigTop;
        //video container
        //////var videoContainerOrigDisplay;
        var videoContainerOrigWidth;
        var videoContainerOrigHeight;
        var videoContainerOrigPosition;
        var videoContainerOrigLeft;
        var videoContainerOrigTop;
        //controllers
        var videoControllersOrigPosition;
        /////var videoControllersOrigMarginBottom;

        //info
        var infoBoxAdjust = 40;
        var infoBoxOrigPosition;

        var videoIsShowHideRunning = false;

        //timer
        var curTime;
        var totalTime;
        var totalTimeInterval;

        // the skins		
        var skins = {
            skin: 'universalBlack',
            initialVolume: 1,
            showInfo: true,
            autoPlayFirstMovie: false,
            autoPlay: true,
            loop: true,
            autoHideControllers: true,
            seekBarAdjust: 255,
            borderWidth: 12,
            borderColor: '#e9e9e9',
            numberOfThumbsPerScreen: 6,
            thumbsReflection: 50,
            isSliderInitialized: false,
            isProgressInitialized: false,

            responsive: false,
            responsiveRelativeToBrowser: false,
            width: 0,//hidden, used for responsive
            height: 0,//hidden, used for responsive
            width100Proc: false,
            height100Proc: false,

            setOrigWidthHeight: true,
            thumbsOnMarginTop: 0,
            origWidth: 0,
            origHeight: 0,
            origThumbW: 0,
            origThumbH: 0,
            origthumbsHolderWrapperH: 121,
            origthumbsHolder_MarginTop: 0,
            windowOrientationScreenSize0: 0,
            windowOrientationScreenSize90: 0,
            windowOrientationScreenSize_90: 0,
            windowCurOrientation: 0

        };
        var options = $.extend(skins, options);

        function getInternetExplorerVersion()
            // -1 - not IE
            // 7,8,9 etc
        {
            var rv = -1; // Return value assumes failure.
            if (navigator.appName == 'Microsoft Internet Explorer') {
                var ua = navigator.userAgent;
                var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
                if (re.exec(ua) != null)
                    rv = parseFloat(RegExp.$1);
            }
            else if (navigator.appName == 'Netscape') {
                var ua = navigator.userAgent;
                var re = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
                if (re.exec(ua) != null)
                    rv = parseFloat(RegExp.$1);
            }
            return parseInt(rv, 10);
        }
        var ver_ie = getInternetExplorerVersion();


        return this.each(function () {
            var vp2_html5_bottomPlaylist_Video = $(this);
            videoID = vp2_html5_bottomPlaylist_Video.attr('id');
            //alert (videoID);		

            var current_obj = {
                windowWidth: 0,
                carouselStep: 0,
                thumbMarginLeft: 0
            };

            //the controllers
            var vp2_html5_bottomPlaylist_controls = $('<div class="VideoControls"><a class="VideoRewind" title="Rewind"></a><a class="VideoPlay" title="Play/Pause"></a><div class="VideoBuffer"></div><div class="VideoSeek"></div><a class="VideoShowHidePlaylist" title="Show/Hide Playlist"></a><a class="VideoInfoBut" title="Info"></a><div class="VideoTimer">00:00</div><div class="VolumeAll"><div class="VolumeSlider"></div><a class="VolumeButton" title="Mute/Unmute"></a></div><a class="VideoFullScreen" title="FullScreen"></a></div> <div class="VideoInfoBox"></div>    <div class="thumbsHolderWrapper"><div class="thumbsHolderVisibleWrapper"><div class="thumbsHolder"></div></div></div>    </div>');





            //the elements
            var vp2_html5_bottomPlaylist_container = vp2_html5_bottomPlaylist_Video.parent('.vp2_html5_bottomPlaylist');
            var vp2_html5_bottomPlaylist_border = vp2_html5_bottomPlaylist_container.parent('.vp2_html5_bottomPlaylistBorder');

            vp2_html5_bottomPlaylist_container.addClass(options.skin);
            vp2_html5_bottomPlaylist_container.append(vp2_html5_bottomPlaylist_controls);

            var vp2_html5_bottomPlaylist_controls = $('.VideoControls', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_info_box = $('.VideoInfoBox', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_rewind_btn = $('.VideoRewind', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_play_btn = $('.VideoPlay', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_Video_buffer = $('.VideoBuffer', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_Video_seek = $('.VideoSeek', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_info_btn = $('.VideoInfoBut', vp2_html5_bottomPlaylist_container);
            if (!options.showInfo)
                vp2_html5_bottomPlaylist_info_btn.addClass("hideElement");
            var vp2_html5_bottomPlaylist_showHidePlaylist_btn = $('.VideoShowHidePlaylist', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_Video_timer = $('.VideoTimer', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_volumeAll = $('.VolumeAll', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_volume = $('.VolumeSlider', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_volume_btn = $('.VolumeButton', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_fullscreen_btn = $('.VideoFullScreen', vp2_html5_bottomPlaylist_container);

            var vp2_html5_bottomPlaylist_thumbsHolderWrapper = $('.thumbsHolderWrapper', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper = $('.thumbsHolderVisibleWrapper', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_thumbsHolder = $('.thumbsHolder', vp2_html5_bottomPlaylist_container);
            var vp2_html5_bottomPlaylist_carouselLeftNav;
            var vp2_html5_bottomPlaylist_carouselRightNav;
            vp2_html5_bottomPlaylist_carouselLeftNav = $('<div class="carouselLeftNav"></div>');
            vp2_html5_bottomPlaylist_carouselRightNav = $('<div class="carouselRightNav"></div>');
            vp2_html5_bottomPlaylist_thumbsHolderWrapper.append(vp2_html5_bottomPlaylist_carouselLeftNav);
            vp2_html5_bottomPlaylist_thumbsHolderWrapper.append(vp2_html5_bottomPlaylist_carouselRightNav);
            vp2_html5_bottomPlaylist_carouselRightNav.css('left', vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() + 'px');


            vp2_html5_bottomPlaylist_thumbsHolder.css('width', vp2_html5_bottomPlaylist_carouselLeftNav.width() + 'px');


            //set controllers size
            vp2_html5_bottomPlaylist_controls.width(vp2_html5_bottomPlaylist_Video.width());

            //set border size
            vp2_html5_bottomPlaylist_border.width(vp2_html5_bottomPlaylist_Video.width() + 2 * options.borderWidth);
            vp2_html5_bottomPlaylist_border.height(vp2_html5_bottomPlaylist_Video.height() + 3 * options.borderWidth + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height());
            vp2_html5_bottomPlaylist_border.css("background", options.borderColor);

            vp2_html5_bottomPlaylist_container.css({
                'width': vp2_html5_bottomPlaylist_Video.width() + 'px',
                'height': vp2_html5_bottomPlaylist_Video.height() + 'px',
                'top': options.borderWidth + 'px',
                'left': options.borderWidth + 'px'
            });
            /*vp2_html5_bottomPlaylist_container.css('top',options.borderWidth+'px');
			vp2_html5_bottomPlaylist_container.css('left',options.borderWidth+'px');*/


            //set seekbar width
            vp2_html5_bottomPlaylist_Video_seek.css('width', vp2_html5_bottomPlaylist_Video[0].offsetWidth - options.seekBarAdjust + 'px');
            vp2_html5_bottomPlaylist_Video_buffer.css('width', vp2_html5_bottomPlaylist_Video_seek.css('width'));

            //set info box
            vp2_html5_bottomPlaylist_info_box.css('width', vp2_html5_bottomPlaylist_Video[0].offsetWidth - infoBoxAdjust + 'px');
            //vp2_html5_bottomPlaylist_info_box.html( '<p class="movieTitle">'+options.movieTitle+'</p><p class="movieDesc">'+options.movieDesc+'</p>');


            vp2_html5_bottomPlaylist_controls.hide(); // the controls are still hidden





            /*original values start*/
            options.origWidth = document.getElementById(videoID).offsetWidth;
            options.origHeight = document.getElementById(videoID).offsetHeight;
            options.width = options.origWidth;
            options.height = options.origHeight;

            videoOrigPosition = vp2_html5_bottomPlaylist_Video.css('position');
            videoOrigLeft = vp2_html5_bottomPlaylist_Video.css('left');
            videoOrigTop = vp2_html5_bottomPlaylist_Video.css('top');

            //////videoContainerOrigDisplay = vp2_html5_bottomPlaylist_container.css('display');
            videoContainerOrigWidth = vp2_html5_bottomPlaylist_container[0].offsetWidth;
            videoContainerOrigHeight = vp2_html5_bottomPlaylist_container[0].offsetHeight;
            videoContainerOrigPosition = vp2_html5_bottomPlaylist_container.css('position');
            videoContainerOrigLeft = vp2_html5_bottomPlaylist_container.css('left');
            videoContainerOrigTop = vp2_html5_bottomPlaylist_container.css('top');


            infoBoxOrigPosition = vp2_html5_bottomPlaylist_info_box.css('position');

            videoControllersOrigPosition = vp2_html5_bottomPlaylist_controls.css('position');
            //////videoControllersOrigMarginBottom = vp2_html5_bottomPlaylist_controls.css( 'bottom');			  

            vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('width', options.origWidth + 'px');

            videoBorderOrigPosition = vp2_html5_bottomPlaylist_border.css('position');
            videoBorderOrigZIndex = vp2_html5_bottomPlaylist_border.css('z-index');
            videoContainerOrigZIndex = vp2_html5_bottomPlaylist_container.css('z-index');
            videoControllersOrigBottom = vp2_html5_bottomPlaylist_controls.css('bottom');

            infoBoxOrigPosition = vp2_html5_bottomPlaylist_info_box.css('position');
            if (options.responsive && (options.width100Proc || options.height100Proc)) {
                options.setOrigWidthHeight = false;
            }



            //body
            /*bodyOrigMargin=$("body").css("margin");
            if (ver_ie!=-1) {
                bodyOrigOverflow=$("html").css("overflow");
            } else {
                bodyOrigOverflow=$("body").css("overflow");
            }	*/
            bodyOrigOverflow = $("html").css("overflow");


            vp2_html5_bottomPlaylist_controls.css('width', '100%');


            /*original values end*/

            var supports_h264_baseline_video = function () {
                var v = document.getElementById(videoID);
                return v.canPlayType('video/mp4; codecs="avc1.42E01E, mp4a.40.2"');
            }


            var detectBrowserAndVideo = function () {
                //activate current
                $(thumbsHolder_Thumbs[current_img_no]).addClass('thumbsHolder_ThumbON');
                //auto scroll carousel if needed
                carouselScroll();

                var currentVideo = playlist_arr[current_img_no]['sources_webm'];
                var val = navigator.userAgent.toLowerCase();



                if (val.indexOf("chrome") != -1 || val.indexOf("msie") != -1 || val.indexOf("safari") != -1 || val.indexOf("android") != -1)
                    currentVideo = playlist_arr[current_img_no]['sources_mp4'];

                //if (val.match(/(iPad)|(iPhone)|(iPod)|(webOS)/i))
                if (val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1)
                    currentVideo = playlist_arr[current_img_no]['sources_mp4'];

                if (val.indexOf("android") != -1)
                    currentVideo = playlist_arr[current_img_no]['sources_mp4'];


                if (ver_ie != -1) {
                    currentVideo = playlist_arr[current_img_no]['sources_mp4'];
                }

                if (val.indexOf("opera") != -1) {
                    currentVideo = playlist_arr[current_img_no]['sources_webm'];
                    if (supports_h264_baseline_video() != '') {
                        currentVideo = playlist_arr[current_img_no]['sources_mp4'];
                    }
                }

                if (val.indexOf("opr/") != -1) {
                    currentVideo = playlist_arr[current_img_no]['sources_webm'];
                    if (supports_h264_baseline_video() != '') {
                        currentVideo = playlist_arr[current_img_no]['sources_mp4'];
                    }
                }

                if (val.indexOf("firefox") != -1 || val.indexOf("mozzila") != -1) {
                    currentVideo = playlist_arr[current_img_no]['sources_webm'];
                    if (supports_h264_baseline_video() != '') {
                        currentVideo = playlist_arr[current_img_no]['sources_mp4'];
                    }
                }

                //var val = this.dataBrowser;
                //alert (currentVideo+ '  --  ' +val);
                return currentVideo;
            };

            //generate playlist
            var isCarouselScrolling = false;
            var currentCarouselLeft = 0;



            var thumbMarginTop = 0;
            var current_img_no = 0;
            var total_images = 0;
            var playlist_arr = new Array();
            var thumbsHolder_Thumb;

            var playlistElements = $('.xplaylist', vp2_html5_bottomPlaylist_Video).children();
            playlistElements.each(function () { // ul-s
                currentElement = $(this);
                total_images++;
                playlist_arr[total_images - 1] = new Array();
                playlist_arr[total_images - 1]['title'] = '';
                playlist_arr[total_images - 1]['desc'] = '';
                playlist_arr[total_images - 1]['thumb'] = '';
                playlist_arr[total_images - 1]['preview'] = '';
                playlist_arr[total_images - 1]['xsources_mp4'] = '';
                playlist_arr[total_images - 1]['xsources_ogv'] = '';
                playlist_arr[total_images - 1]['xsources_webm'] = '';
                playlist_arr[total_images - 1]['xsources_mp4v'] = '';

                //alert (currentElement.find('.xdesc').html())
                if (currentElement.find('.xtitle').html() != null) {
                    playlist_arr[total_images - 1]['title'] = currentElement.find('.xtitle').html();
                }

                if (currentElement.find('.xdesc').html() != null) {
                    playlist_arr[total_images - 1]['desc'] = currentElement.find('.xdesc').html();
                }

                if (currentElement.find('.xthumb').html() != null) {
                    playlist_arr[total_images - 1]['thumb'] = currentElement.find('.xthumb').html();
                }

                if (currentElement.find('.xpreview').html() != null) {
                    playlist_arr[total_images - 1]['preview'] = currentElement.find('.xpreview').html();
                }


                if (currentElement.find('.xsources_mp4').html() != null) {
                    playlist_arr[total_images - 1]['sources_mp4'] = currentElement.find('.xsources_mp4').html();
                }

                if (currentElement.find('.xsources_ogv').html() != null) {
                    playlist_arr[total_images - 1]['sources_ogv'] = currentElement.find('.xsources_ogv').html();
                }

                if (currentElement.find('.xsources_webm').html() != null) {
                    playlist_arr[total_images - 1]['sources_webm'] = currentElement.find('.xsources_webm').html();
                }

                if (currentElement.find('.xsources_mp4v').html() != null) {
                    playlist_arr[total_images - 1]['sources_mp4v'] = currentElement.find('.xsources_mp4v').html();
                }

                thumbsHolder_Thumb = $('<div class="thumbsHolder_ThumbOFF" rel="' + (total_images - 1) + '"><img src="' + playlist_arr[total_images - 1]['thumb'] + '"></div>');
                vp2_html5_bottomPlaylist_thumbsHolder.append(thumbsHolder_Thumb);



                current_obj.thumbMarginLeft = Math.floor((vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() - thumbsHolder_Thumb.width() * options.numberOfThumbsPerScreen) / (options.numberOfThumbsPerScreen - 1));
                //alert (current_obj.thumbMarginLeft);
                vp2_html5_bottomPlaylist_thumbsHolder.css('width', vp2_html5_bottomPlaylist_thumbsHolder.width() + current_obj.thumbMarginLeft + thumbsHolder_Thumb.width() + 'px');
                //alert (current_obj.thumbMarginLeft+' - '+vp2_html5_bottomPlaylist_thumbsHolderWrapper.width()+' - '+vp2_html5_bottomPlaylist_carouselLeftNav.width()+' - '+vp2_html5_bottomPlaylist_carouselRightNav.width()+' - '+thumbsHolder_Thumb.width()+' - '+options.numberOfThumbsPerScreen);
                if (total_images <= 1) {
                    thumbsHolder_Thumb.css('margin-left', Math.floor((vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() - (current_obj.thumbMarginLeft + thumbsHolder_Thumb.width()) * (options.numberOfThumbsPerScreen - 1) - thumbsHolder_Thumb.width()) / 2) + 'px');
                } else {
                    thumbsHolder_Thumb.css('margin-left', current_obj.thumbMarginLeft + 'px');
                }


                options.origThumbW = thumbsHolder_Thumb.width();
                options.origThumbH = thumbsHolder_Thumb.height();
                options.origthumbsHolderWrapperH = vp2_html5_bottomPlaylist_thumbsHolderWrapper.height();

                thumbsHolder_MarginTop = parseInt((vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() - parseInt(thumbsHolder_Thumb.css('height').substring(0, thumbsHolder_Thumb.css('height').length - 2))) / 2);
            });

            vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.css('width', vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.width() - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width());
            vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.css('left', vp2_html5_bottomPlaylist_carouselLeftNav.width());

            current_obj.carouselStep = (thumbsHolder_Thumb.width() + current_obj.thumbMarginLeft) * options.numberOfThumbsPerScreen;
            //disable left nav
            if (Math.floor(current_img_no / options.numberOfThumbsPerScreen) === 0)
                vp2_html5_bottomPlaylist_carouselLeftNav.addClass('carouselLeftNavDisabled');

            //disable right nav
            if (Math.floor(current_img_no / options.numberOfThumbsPerScreen) == Math.floor(total_images / options.numberOfThumbsPerScreen))
                vp2_html5_bottomPlaylist_carouselRightNav.addClass('carouselRightNavDisabled');

            vp2_html5_bottomPlaylist_thumbsHolderWrapper.css("top", vp2_html5_bottomPlaylist_Video[0].offsetHeight + options.borderWidth + 'px');

            if (options.thumbsReflection > 0)
                thumbsHolder_MarginTop = thumbsHolder_MarginTop - 7;
            var img_inside = $('.thumbsHolder_ThumbOFF', vp2_html5_bottomPlaylist_container).find('img:first');
            img_inside.css("margin-top", thumbsHolder_MarginTop + "px");

            //create reflection
            var op = { opacity: options.thumbsReflection / 100 };
            if (options.thumbsReflection > 0) {
                img_inside.reflect(op);
            }

            thumbsHolder_Thumb.css('background-position', 'center ' + options.thumbsOnMarginTop + 'px');


            //carousel controllers
            vp2_html5_bottomPlaylist_carouselLeftNav.click(function ()
            {
                
                if (!isCarouselScrolling) {
                    currentCarouselLeft = vp2_html5_bottomPlaylist_thumbsHolder.css('left').substr(0, vp2_html5_bottomPlaylist_thumbsHolder.css('left').lastIndexOf('px'));

                    if (currentCarouselLeft < 0)
                        carouselScroll(1);
                }
            });


            vp2_html5_bottomPlaylist_carouselRightNav.click(function ()
            {
               
                if (!isCarouselScrolling) {
                    currentCarouselLeft = vp2_html5_bottomPlaylist_thumbsHolder.css('left').substr(0, vp2_html5_bottomPlaylist_thumbsHolder.css('left').lastIndexOf('px'));
                    if (Math.abs(currentCarouselLeft - current_obj.carouselStep) < (thumbsHolder_Thumb.width() + current_obj.thumbMarginLeft) * total_images)
                        carouselScroll(-1);
                }
            });



            var carouselScroll = function (direction) {
                currentCarouselLeft = vp2_html5_bottomPlaylist_thumbsHolder.css('left').substr(0, vp2_html5_bottomPlaylist_thumbsHolder.css('left').lastIndexOf('px'));
                if (direction === 1 || direction === -1) {
                    isCarouselScrolling = true;
                    vp2_html5_bottomPlaylist_thumbsHolder.css('opacity', '0.5');
                    vp2_html5_bottomPlaylist_thumbsHolder.animate({
                        opacity: 1,
                        left: '+=' + direction * current_obj.carouselStep
                    }, 500, 'easeOutCubic', function () {
                        // Animation complete.
                        disableCarouselNav();
                        isCarouselScrolling = false;
                    });
                } else {

                    if (currentCarouselLeft != (-1) * Math.floor(current_img_no / options.numberOfThumbsPerScreen) * current_obj.carouselStep) {
                        isCarouselScrolling = true;
                        vp2_html5_bottomPlaylist_thumbsHolder.css('opacity', '0.5');
                        vp2_html5_bottomPlaylist_thumbsHolder.animate({
                            opacity: 1,
                            left: (-1) * Math.floor(current_img_no / options.numberOfThumbsPerScreen) * current_obj.carouselStep
                        }, 500, 'easeOutCubic', function () {
                            // Animation complete.
                            disableCarouselNav();
                            isCarouselScrolling = false;
                        });
                    }
                }


            };

            var disableCarouselNav = function () {
                currentCarouselLeft = vp2_html5_bottomPlaylist_thumbsHolder.css('left').substr(0, vp2_html5_bottomPlaylist_thumbsHolder.css('left').lastIndexOf('px'));
                //alert (currentCarouselLeft)
                if (currentCarouselLeft < 0) {
                    if (vp2_html5_bottomPlaylist_carouselLeftNav.hasClass('carouselLeftNavDisabled'))
                        vp2_html5_bottomPlaylist_carouselLeftNav.removeClass('carouselLeftNavDisabled');
                } else {
                    vp2_html5_bottomPlaylist_carouselLeftNav.addClass('carouselLeftNavDisabled');
                }

                if (Math.abs(currentCarouselLeft - current_obj.carouselStep) < (thumbsHolder_Thumb.width() + current_obj.thumbMarginLeft) * total_images) {
                    if (vp2_html5_bottomPlaylist_carouselRightNav.hasClass('carouselRightNavDisabled'))
                        vp2_html5_bottomPlaylist_carouselRightNav.removeClass('carouselRightNavDisabled');
                } else {
                    vp2_html5_bottomPlaylist_carouselRightNav.addClass('carouselRightNavDisabled');
                }
            };







            //tumbs nav
            var thumbsHolder_Thumbs = $('.thumbsHolder_ThumbOFF', vp2_html5_bottomPlaylist_container);
            thumbsHolder_Thumbs.click(function ()
            {
                
                var currentBut = $(this);
                var i = currentBut.attr('rel');

               // alert(playlist_arr[i]['sources_mp4']);

                var filmId = playlist_arr[i]['sources_mp4'];
                //location.href = '/Guest/ShowFilm/' + filmId;
                //return true;


                $.ajax({
                    url: "/Guest/ShowFilmAjax/",
                    data: { id: filmId },
                dataType: "html",
                type: "POST",
                error: function ()
                {
                    alert("An error occurred.");
                },
                success: function (data)
                {
                    $("#MoviePlayer").html("");
                    $("#MoviePlayer").html(data);
                    return true;
                }
                });

                return true;
                

                if (current_img_no != i) {

                    //alert (current_img_no+'  --  '+i)
                    //deactivate previous 
                    $(thumbsHolder_Thumbs[current_img_no]).removeClass('thumbsHolder_ThumbON');

                    current_img_no = i;
                    changeSrcAndPoster(options.autoPlay);
                }
            });

            thumbsHolder_Thumbs.mouseenter(function () {
                var currentBut = $(this);
                var i = currentBut.attr('rel');

                currentBut.addClass('thumbsHolder_ThumbON');
            });

            thumbsHolder_Thumbs.mouseleave(function () {
                var currentBut = $(this);
                var i = currentBut.attr('rel');

                if (current_img_no != i)
                    currentBut.removeClass('thumbsHolder_ThumbON');
            });

            thumbsHolder_Thumbs.dblclick(function (event)
            {
                
                var currentBut = $(this);
                var i = currentBut.attr('rel');
                var filmId = playlist_arr[i]['sources_mp4'];
                //location.href = '/Guest/ShowFilm/' + filmId;
                //return true;

                $.ajax({
                    url: "/Guest/ShowFilmAjax/",
                    data: { id: filmId },
                    dataType: "html",
                    type: "POST",
                    error: function () {
                        alert("An error occurred.");
                    },
                    success: function (data) {
                        $("#MoviePlayer").html("");
                        $("#MoviePlayer").html(data);
                        return true;
                    }
                });

                //currentBut.addClass('thumbsHolder_ThumbON');
            });/**/


            var changeSrcAndPoster = function (auto_play) {
                //seekbar init
                if (options.isSliderInitialized) {
                    vp2_html5_bottomPlaylist_Video_seek.slider("destroy");
                    options.isSliderInitialized = false;
                }

                if (options.isProgressInitialized) {
                    vp2_html5_bottomPlaylist_Video_buffer.progressbar("destroy");
                    vp2_html5_bottomPlaylist_Video.unbind('progress');
                    options.isProgressInitialized = false;
                }



                totalTimeInterval = 'Infinity';

                document.getElementById(videoID).poster = playlist_arr[current_img_no]['preview'];
                //info
                vp2_html5_bottomPlaylist_info_box.html('<p class="movieTitle">' + playlist_arr[current_img_no]['title'] + '</p><p class="movieDesc">' + playlist_arr[current_img_no]['desc'] + '</p>');

                /*document.getElementById(videoID).src = null;
       			document.getElementById(videoID).load();*/

                document.getElementById(videoID).src = detectBrowserAndVideo();
                document.getElementById(videoID).load();

                var val = navigator.userAgent.toLowerCase();
                if (val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1 || val.indexOf("android") != -1) {
                    if (auto_play) {
                        document.getElementById(videoID).play();
                        vp2_html5_bottomPlaylist_play_btn.addClass('VideoPause');
                    } else {
                        vp2_html5_bottomPlaylist_play_btn.removeClass('VideoPause');
                    }
                    generate_seekBar();
                } else {
                    clearInterval(totalTimeInterval);
                    totalTimeInterval = setInterval(function () {
                        //alert (document.getElementById(videoID).readyState);
                        //alert (document.getElementById(videoID).duration);
                        if (document.getElementById(videoID).readyState > 0 && !isNaN(document.getElementById(videoID).duration) && document.getElementById(videoID).duration != 'Infinity') {
                            totalTime = document.getElementById(videoID).duration;
                            //alert ("totalTime1: "+totalTime);					
                            generate_seekBar();
                            if (auto_play) {
                                document.getElementById(videoID).play();
                                vp2_html5_bottomPlaylist_play_btn.addClass('VideoPause');
                            } else {
                                vp2_html5_bottomPlaylist_play_btn.removeClass('VideoPause');
                            }

                            clearInterval(totalTimeInterval);
                        }

                    }, 700);
                }

            };

            //initialize first video
            changeSrcAndPoster(options.autoPlayFirstMovie);




            /* rewind */
            vp2_html5_bottomPlaylist_rewind_btn.click(function () {
                document.getElementById(videoID).currentTime = 0;
                document.getElementById(videoID).play();
            });
            /* play/pause*/
            var vp2_html5_bottomPlaylist_PlayPause = function () {
                if (document.getElementById(videoID).paused == false) {
                    document.getElementById(videoID).pause();
                } else {
                    document.getElementById(videoID).play();
                }
            };

            vp2_html5_bottomPlaylist_play_btn.click(vp2_html5_bottomPlaylist_PlayPause);
            vp2_html5_bottomPlaylist_Video.click(vp2_html5_bottomPlaylist_PlayPause);

            vp2_html5_bottomPlaylist_Video.bind('play', function () {
                vp2_html5_bottomPlaylist_play_btn.addClass('VideoPause');
            });

            vp2_html5_bottomPlaylist_Video.bind('pause', function () {
                vp2_html5_bottomPlaylist_play_btn.removeClass('VideoPause');
            });

            /*vp2_html5_bottomPlaylist_Video.bind('ended', function() {
				vp2_html5_bottomPlaylist_play_btn.removeClass('VideoPause');
			});*/


            var val = navigator.userAgent.toLowerCase();
            if (val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1) {
                //nothing
            } else {
                //show controllers on mouser over / hide controllers on mouse out
                vp2_html5_bottomPlaylist_container.mouseover(function () {
                    if (options.autoHideControllers) {
                        vp2_html5_bottomPlaylist_controls.show();
                    }
                });
                vp2_html5_bottomPlaylist_container.mouseout(function () {
                    if (options.autoHideControllers) {
                        if (vp2_html5_bottomPlaylist_volumeAll.css('height').substring(0, vp2_html5_bottomPlaylist_volumeAll.css('height').length - 2) < 120) {
                            vp2_html5_bottomPlaylist_controls.hide();
                        }
                    }
                });
            }
            //play/pause using spacebar
            vp2_html5_bottomPlaylist_container.keydown(function (evt) {
                if (evt.keyCode == 32)
                    vp2_html5_bottomPlaylist_PlayPause();
            });



            //fullscreen
            var fullScreenOn = function () {
                videoIsFullScreen = true;
                //change button
                vp2_html5_bottomPlaylist_fullscreen_btn.removeClass('VideoFullScreen');
                vp2_html5_bottomPlaylist_fullscreen_btn.addClass('VideoFullScreenIn');
                //preserve original/nonFullScreen values
                //body

                // $("body").css("overflow", "hidden");
                $("html").css("overflow", "hidden");
                $("body").css("margin", 0);
                $(".vp2_html5_bottomPlaylist").css('display', 'none');
                vp2_html5_bottomPlaylist_container.css('display', 'block');

                var new_height;
                var new_videoControllersBottom;
                if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') != 'none') {
                    new_height = window.innerHeight - vp2_html5_bottomPlaylist_thumbsHolderWrapper.height();
                    new_videoControllersBottom = window.innerHeight - new_height + 'px';
                } else {
                    new_height = window.innerHeight;
                    new_videoControllersBottom = videoControllersOrigBottom;
                }

                //video
                vp2_html5_bottomPlaylist_Video.css('position', 'fixed');
                vp2_html5_bottomPlaylist_Video.css('width', window.innerWidth + "px");
                vp2_html5_bottomPlaylist_Video.css('height', new_height + "px");
                vp2_html5_bottomPlaylist_Video.css('top', 0);
                vp2_html5_bottomPlaylist_Video.css('left', 0);
                //container
                //vp2_html5_bottomPlaylist_container.css( 'position', 'absolute' );
                vp2_html5_bottomPlaylist_container.css('position', 'fixed');
                //vp2_html5_bottomPlaylist_container.css( 'width', window.innerWidth + "px" );
                //vp2_html5_bottomPlaylist_container.css( 'height', window.innerWidth + "px" );
                vp2_html5_bottomPlaylist_container.css('width', "100%");
                vp2_html5_bottomPlaylist_container.css('height', "20000px");
                vp2_html5_bottomPlaylist_container.css('top', 0);
                vp2_html5_bottomPlaylist_container.css('left', 0);
                vp2_html5_bottomPlaylist_container.css('z-index', 10000);
                //controller
                vp2_html5_bottomPlaylist_controls.css('position', 'fixed');
                vp2_html5_bottomPlaylist_controls.css('bottom', new_videoControllersBottom);
                vp2_html5_bottomPlaylist_Video_seek.css('width', window.innerWidth - options.seekBarAdjust + 'px');
                vp2_html5_bottomPlaylist_Video_buffer.css('width', vp2_html5_bottomPlaylist_Video_seek.css('width'));
                //info box
                vp2_html5_bottomPlaylist_info_box.css('position', 'fixed');
                vp2_html5_bottomPlaylist_info_box.css('width', window.innerWidth - infoBoxAdjust + 'px');
                if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') != 'none') {
                    vp2_html5_bottomPlaylist_info_box.css('margin-bottom', vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + 'px');
                }
                //playlist
                //alert (vp2_html5_bottomPlaylist_Video[0].offsetWidth+ '   -   '+ vp2_html5_bottomPlaylist_Video.width())
                vp2_html5_bottomPlaylist_thumbsHolderWrapper.css("top", window.innerHeight - vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + 'px');
                vp2_html5_bottomPlaylist_thumbsHolderWrapper.css("margin-left", Math.round((window.innerWidth - options.origWidth) / 2));

                //document.getElementById(videoID).webkitEnterFullscreen();
                //document.getElementById(videoID).mozRequestFullScreen();
                //alert ("aa: "+videoIsFullScreen);
            };
            var fullScreenOff = function () {
                videoIsFullScreen = false;
                //change button
                vp2_html5_bottomPlaylist_fullscreen_btn.removeClass('VideoFullScreenIn');
                vp2_html5_bottomPlaylist_fullscreen_btn.addClass('VideoFullScreen');


                //body
                $("body").css("margin", bodyOrigMargin);
                //$("body").css("overflow", bodyOrigOverflow);
                $("html").css("overflow", bodyOrigOverflow);
                $(".vp2_html5_bottomPlaylist").css('display', 'block');


                //border
                vp2_html5_bottomPlaylist_border.css('position', videoBorderOrigPosition);
                vp2_html5_bottomPlaylist_border.css('width', options.origWidth + 2 * options.borderWidth + "px");
                vp2_html5_bottomPlaylist_border.css('height', options.origHeight + 3 * options.borderWidth + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + "px");
                vp2_html5_bottomPlaylist_border.css('background', options.borderColor);
                vp2_html5_bottomPlaylist_border.css('z-index', videoBorderOrigZIndex);


                //container
                vp2_html5_bottomPlaylist_container.css('position', videoContainerOrigPosition);
                vp2_html5_bottomPlaylist_container.css('width', videoContainerOrigWidth + "px");
                vp2_html5_bottomPlaylist_container.css('height', videoContainerOrigHeight + "px");
                vp2_html5_bottomPlaylist_container.css('top', videoContainerOrigTop);
                vp2_html5_bottomPlaylist_container.css('left', videoContainerOrigLeft);
                vp2_html5_bottomPlaylist_container.css('z-index', videoContainerOrigZIndex);


                vp2_html5_bottomPlaylist_container.css('top', videoContainerOrigTop);
                vp2_html5_bottomPlaylist_container.css('left', videoContainerOrigLeft);

                //controllers
                vp2_html5_bottomPlaylist_controls.css('position', videoControllersOrigPosition);
                vp2_html5_bottomPlaylist_controls.css({
                    'bottom': 0,
                    'margin-bottom': 0
                });
                vp2_html5_bottomPlaylist_Video_seek.css('width', options.origWidth - options.seekBarAdjust + 'px');
                vp2_html5_bottomPlaylist_Video_buffer.css('width', vp2_html5_bottomPlaylist_Video_seek.css('width'));
                //info box
                vp2_html5_bottomPlaylist_info_box.css('width', options.origWidth - infoBoxAdjust + 'px');
                vp2_html5_bottomPlaylist_info_box.css('position', infoBoxOrigPosition);
                vp2_html5_bottomPlaylist_info_box.css('margin-bottom', 0 + 'px');


                //video
                var new_height;
                vp2_html5_bottomPlaylist_Video.css('position', videoOrigPosition);
                vp2_html5_bottomPlaylist_Video.css('width', options.origWidth + "px");
                if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') != 'none') {
                    new_height = options.origHeight;
                } else {
                    new_height = options.origHeight + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + options.borderWidth;
                }
                vp2_html5_bottomPlaylist_Video.css('height', new_height + "px");
                vp2_html5_bottomPlaylist_container.css('height', new_height + "px");


                vp2_html5_bottomPlaylist_Video.css('top', videoOrigTop);
                vp2_html5_bottomPlaylist_Video.css('left', videoOrigLeft);


                //playlist
                vp2_html5_bottomPlaylist_thumbsHolderWrapper.css("margin-left", 0);
                vp2_html5_bottomPlaylist_thumbsHolderWrapper.css("top", vp2_html5_bottomPlaylist_Video[0].offsetHeight + options.borderWidth + 'px');

            };



            var handleFullScreen = function () {
                if (!videoIsFullScreen) {
                    fullScreenOn();
                } else {
                    fullScreenOff();
                }
            };

            /*document.onkeydown = function(evt) {
			    evt = evt || window.event;
			    if (evt.keyCode == 27 && videoIsFullScreen) {
			        //alert("Escape");
			    	fullScreenOff();
			    }
			};	*/

            vp2_html5_bottomPlaylist_container.dblclick(function () {
                if (!videoIsFullScreen) {
                    vp2_html5_bottomPlaylist_fullscreen_btn.click();
                }
            });

            vp2_html5_bottomPlaylist_fullscreen_btn.click(function () {
                if (screenfull.enabled) {
                    vp2_html5_bottomPlaylist_container.css('display', 'none');
                    //screenfull.toggle(document.getElementById( vp2_html5_bottomPlaylist_container.attr("id") ));
                    screenfull.toggle(vp2_html5_bottomPlaylist_border[0]);
                    //screenfull.toggle(document.body);
                    screenfull.onchange = function (e) {

                        //rearange thumbs area if it was resized	
                        if (options.responsive && options.width != options.origWidth && !videoIsFullScreen) {
                            options.width = options.origWidth;
                            options.height = options.origHeight;

                            vp2_html5_bottomPlaylist_Video.css('width', options.width + "px");
                            vp2_html5_bottomPlaylist_Video.css('height', options.height + "px");
                            //container
                            vp2_html5_bottomPlaylist_container.css('width', options.width + "px");
                            vp2_html5_bottomPlaylist_container.css('height', options.height + "px");
                            //controller
                            vp2_html5_bottomPlaylist_controls.css('bottom', videoControllersOrigBottom);
                            vp2_html5_bottomPlaylist_Video_seek.css('width', options.width - options.seekBarAdjust + 'px');
                            vp2_html5_bottomPlaylist_Video_buffer.css('width', vp2_html5_bottomPlaylist_Video_seek.css('width'));
                            //info box
                            vp2_html5_bottomPlaylist_info_box.css('width', options.width - infoBoxAdjust + 'px');
                            rearangethumbs();
                        }



                        //alert (screenfull.isFullscreen);
                        setTimeout(function () { handleFullScreen() }, 50);
                        //handleFullScreen();
                    };
                } else {
                    //var ver_iex=getInternetExplorerVersion();
                    if ((ver_ie != -1 || val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1) && options.responsive && options.responsiveRelativeToBrowser && options.width100Proc && options.height100Proc) {
                        //nothing
                    } else {
                        handleFullScreen();
                    }
                }

            });




            // timer mouse over
            var is_overTimer = false;
            vp2_html5_bottomPlaylist_Video_timer.mouseover(function () {
                is_overTimer = true;
                curTime = document.getElementById(videoID).currentTime;
                totalTime = document.getElementById(videoID).duration;
                vp2_html5_bottomPlaylist_Video_timer.text('-' + vp2_html5_bottomPlaylist_FormatTime(totalTime - curTime));
            });
            vp2_html5_bottomPlaylist_Video_timer.mouseout(function () {
                is_overTimer = false;
                curTime = document.getElementById(videoID).currentTime;
                totalTime = document.getElementById(videoID).duration;
                vp2_html5_bottomPlaylist_Video_timer.text(vp2_html5_bottomPlaylist_FormatTime(curTime));
            });




            var is_seeking = false;
            function generate_seekBar()
            {
                //alert ("gen");
                if (document.getElementById(videoID).readyState) {
                    totalTime = document.getElementById(videoID).duration;
                    vp2_html5_bottomPlaylist_Video_seek.slider({
                        value: 0,
                        step: 0.01,
                        orientation: "horizontal",
                        range: "min",
                        max: totalTime,
                        animate: true,
                        slide: function () {
                            is_seeking = true;
                        },
                        stop: function (e, ui) {
                            is_seeking = false;
                            document.getElementById(videoID).currentTime = ui.value;
                        },
                        create: function (e, ui) {
                            options.isSliderInitialized = true;
                        }
                    });


                    var bufferedTime = 0;
                    vp2_html5_bottomPlaylist_Video_buffer.progressbar({
                        value: bufferedTime,
                        create: function (e, ui) {
                            options.isProgressInitialized = true;
                        }
                    });

                    vp2_html5_bottomPlaylist_Video.bind('progress', function () {

                        //alert(document.getElementById(videoID).buffered.end(document.getElementById(videoID).buffered.length-1));

                        try
                        {
                            bufferedTime = document.getElementById(videoID).buffered.end(document.getElementById(videoID).buffered.length - 1);

                            //if(bufferedTime == 0)
                            //{
                            //    alert (bufferedTime);
                            //}
                            
                        }
                        catch (e)
                        {
                            //alert(e.message);
                            //alert(bufferedTime);
                        }

                        //alert (bufferedTime);

                        if (bufferedTime > 0)
                        {
                            vp2_html5_bottomPlaylist_Video_buffer.progressbar({ value: bufferedTime * vp2_html5_bottomPlaylist_Video_buffer.css('width').substring(0, vp2_html5_bottomPlaylist_Video_buffer.css('width').length - 2) / totalTime });
                        }
                    });

                    vp2_html5_bottomPlaylist_controls.show();

                } else
                {
                    setTimeout(generate_seekBar, 200);
                }
            };

            generate_seekBar();

            var vp2_html5_bottomPlaylist_FormatTime = function (seconds) {
                var m = Math.floor(seconds / 60) < 10 ? "0" + Math.floor(seconds / 60) : Math.floor(seconds / 60);
                var s = Math.floor(seconds - (m * 60)) < 10 ? "0" + Math.floor(seconds - (m * 60)) : Math.floor(seconds - (m * 60));
                return m + ":" + s;
            };

            var seekUpdate = function () {
                if (options.isSliderInitialized) {
                    curTime = document.getElementById(videoID).currentTime;
                    totalTime = document.getElementById(videoID).duration;
                    if (!is_seeking) vp2_html5_bottomPlaylist_Video_seek.slider('value', curTime);
                    if (!is_overTimer)
                        vp2_html5_bottomPlaylist_Video_timer.text(vp2_html5_bottomPlaylist_FormatTime(curTime));
                    else
                        vp2_html5_bottomPlaylist_Video_timer.text('-' + vp2_html5_bottomPlaylist_FormatTime(totalTime - curTime));
                }
            };

            vp2_html5_bottomPlaylist_Video.bind('timeupdate', seekUpdate);

            //info
            //var vp2_html5_bottomPlaylist_info_btn = $('.VideoInfoBut', vp2_html5_bottomPlaylist_container);
            vp2_html5_bottomPlaylist_info_btn.click(function () {
                if (options.skin == "giant") {
                    if (vp2_html5_bottomPlaylist_info_box.css("display") == "none")
                        vp2_html5_bottomPlaylist_info_box.fadeIn();
                    else
                        vp2_html5_bottomPlaylist_info_box.fadeOut();
                } else {
                    vp2_html5_bottomPlaylist_info_box.slideToggle(300);
                }
            });

            //show/hide playlist
            vp2_html5_bottomPlaylist_showHidePlaylist_btn.click(function () {
                if (!videoIsShowHideRunning) {
                    videoIsShowHideRunning = true;
                    if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') != 'none') { //hide
                        var new_height;
                        new_height = vp2_html5_bottomPlaylist_Video[0].offsetHeight + options.borderWidth + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height();
                        /*if (videoIsFullScreen)
							new_width=new_width+options.borderWidth;*/

                        vp2_html5_bottomPlaylist_showHidePlaylist_btn.addClass('VideoShowHidePlaylist_onlyShow');

                        if (videoIsFullScreen) {
                            //vp2_html5_bottomPlaylist_controls.css( 'margin-bottom', vp2_html5_bottomPlaylist_thumbsHolderWrapper.height()+'px');
                            vp2_html5_bottomPlaylist_controls.css({
                                'bottom': 0,
                                'margin-bottom': 0
                            });

                            vp2_html5_bottomPlaylist_info_box.css('margin-bottom', 0);
                        } else {
                            vp2_html5_bottomPlaylist_container.css('height', new_height + "px");
                        }

                        vp2_html5_bottomPlaylist_thumbsHolderWrapper.animate({
                            opacity: 0
                        }, 150, function () {
                            vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display', 'none');
                        }
						);




                        vp2_html5_bottomPlaylist_Video.animate({
                            height: new_height
                        }, 300, function () {
                            // Animation complete.
                            videoIsShowHideRunning = false;
                        }
						);
                    } else { //show
                        new_height = vp2_html5_bottomPlaylist_Video[0].offsetHeight - options.borderWidth - vp2_html5_bottomPlaylist_thumbsHolderWrapper.height();
                        /*if (videoIsFullScreen)
							new_width=new_width-options.borderWidth;	*/
                        vp2_html5_bottomPlaylist_showHidePlaylist_btn.removeClass('VideoShowHidePlaylist_onlyShow');

                        if (videoIsFullScreen) {
                            vp2_html5_bottomPlaylist_controls.css('margin-bottom', vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + 'px');
                            vp2_html5_bottomPlaylist_info_box.css('margin-bottom', vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() + 'px');
                        } else {
                            //alert (vp2_html5_bottomPlaylist_controls.css( 'margin-bottom') );
                            //vp2_html5_bottomPlaylist_container.css( 'height', parseInt((videoContainerOrigHeight/(options.origWidth/options.width)),10) + "px" );
                            vp2_html5_bottomPlaylist_container.css('height', options.height + "px");

                        }

                        vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display', 'block');
                        vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('top', new_height + options.borderWidth + 'px');
                        vp2_html5_bottomPlaylist_thumbsHolderWrapper.animate({
                            opacity: 1
                        }, 600, function () {
                            // Animation complete.
                            videoIsShowHideRunning = false;
                            carouselScroll();
                        }
						);

                        vp2_html5_bottomPlaylist_Video.animate({
                            height: new_height
                        }, 300, function () {
                            // Animation complete.
                        }
						);
                    }
                }
            });


            //movie ended
            vp2_html5_bottomPlaylist_Video[0].addEventListener('ended', endMovieHandler, false);
            function endMovieHandler(e) {
                if (!e) { e = window.event; }
                // What you want to do after the event
                //alert ("ended");
                if (options.loop && val.indexOf("android") == -1) {
                    //alert (current_img_no);
                    //deactivate previous
                    $(thumbsHolder_Thumbs[current_img_no]).removeClass('thumbsHolder_ThumbON');

                    if (current_img_no == total_images - 1)
                        current_img_no = 0;
                    else
                        current_img_no++;

                    changeSrcAndPoster(options.autoPlay);
                }
            }




            var vp2_html5_bottomPlaylist_VolumeValue = 1;
            vp2_html5_bottomPlaylist_volume.slider({
                value: options.initialVolume,
                orientation: "vertical",
                range: "min",
                max: 1,
                step: 0.05,
                animate: true,
                slide: function (e, ui) {
                    document.getElementById(videoID).muted = false;
                    vp2_html5_bottomPlaylist_VolumeValue = ui.value;
                    document.getElementById(videoID).volume = ui.value;
                }
            });
            document.getElementById(videoID).volume = options.initialVolume;

            var muteVolume = function () {
                if (document.getElementById(videoID).muted == true) {
                    document.getElementById(videoID).muted = false;
                    vp2_html5_bottomPlaylist_volume.slider('value', vp2_html5_bottomPlaylist_VolumeValue);

                    vp2_html5_bottomPlaylist_volume_btn.removeClass('VolumeButtonMute');
                    vp2_html5_bottomPlaylist_volume_btn.addClass('VolumeButton');
                } else {
                    document.getElementById(videoID).muted = true;
                    vp2_html5_bottomPlaylist_volume.slider('value', '0');

                    vp2_html5_bottomPlaylist_volume_btn.removeClass('VolumeButton');
                    vp2_html5_bottomPlaylist_volume_btn.addClass('VolumeButtonMute');
                };
            };

            vp2_html5_bottomPlaylist_volume_btn.click(muteVolume);

            vp2_html5_bottomPlaylist_Video.removeAttr('controls');





            var rearangethumbs = function () {
                //thumbs

                vp2_html5_bottomPlaylist_thumbsHolderWrapper.css({
                    "top": vp2_html5_bottomPlaylist_Video[0].offsetHeight + options.borderWidth + 'px',
                    "width": options.width + 'px',
                    "height": parseInt(options.origthumbsHolderWrapperH / (options.origWidth / options.width), 10) + "px"
                });

                thumbIconCorrection = -7;
                bgTopCorrection = 0;
                if (options.origWidth / options.width == 1 && options.thumbsReflection > 0) {
                    bgTopCorrection = -7;
                    thumbIconCorrection = 0;
                }

                vp2_html5_bottomPlaylist_carouselLeftNav.css('background-position', '0px ' + ((vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() - options.origthumbsHolderWrapperH) / 2 + bgTopCorrection + 3) + 'px');
                vp2_html5_bottomPlaylist_carouselRightNav.css('background-position', '0px ' + ((vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() - options.origthumbsHolderWrapperH) / 2 + bgTopCorrection + 3) + 'px');
                vp2_html5_bottomPlaylist_carouselRightNav.css('left', vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() + 'px');
                vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.css('width', options.width - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width());
                options.origWidthThumbsHolderVisibleWrapper = options.origWidth - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width();


                thumbsHolder_Thumbs.css({
                    'width': parseInt(options.origThumbW / (options.origWidthThumbsHolderVisibleWrapper / vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.width()), 10) + 'px',
                    'height': parseInt(options.origThumbH / (options.origWidthThumbsHolderVisibleWrapper / vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.width()), 10) + 'px'
                });

                if (options.numberOfThumbsPerScreen >= total_images) {
                    vp2_html5_bottomPlaylist_thumbsHolderVisibleWrapper.css('left', parseInt((vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - (thumbsHolder_Thumb.width() + current_obj.thumbMarginLeft) * total_images) / 2, 10) + parseInt(current_obj.thumbMarginLeft / 2, 10) + 'px');
                }


                var imageInside = $('.thumbsHolder_ThumbOFF', vp2_html5_bottomPlaylist_container).find('img:first');
                if (options.thumbsReflection > 0) {
                    imageInside.unreflect();
                }
                imageInside.css({
                    "width": thumbsHolder_Thumbs.width() + "px",
                    "height": thumbsHolder_Thumbs.height() + "px",
                    "margin-top": parseInt((vp2_html5_bottomPlaylist_thumbsHolderWrapper.height() - thumbsHolder_Thumbs.height()) / 2, 10) + bgTopCorrection + "px"
                });
                //img_inside.css("margin-top",thumbsHolder_MarginTop+"px");

                //create reflection
                if (options.origWidth / options.width == 1 && options.thumbsReflection > 0) {
                    var op = { opacity: options.thumbsReflection / 100 };
                    imageInside.reflect(op);
                }


                //alert ( imageInside.css('margin-top')+' ---'+imageInside.css('margin-top').substring(0, imageInside.css('margin-top').length-2) );
                current_obj.thumbMarginLeft = Math.floor((vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() - thumbsHolder_Thumb.width() * options.numberOfThumbsPerScreen) / (options.numberOfThumbsPerScreen - 1));
                thumb_i = -1;
                vp2_html5_bottomPlaylist_thumbsHolder.children().each(function () {
                    thumb_i++;
                    theThumb = $(this);

                    //theThumb.css('background-position','center '+parseInt((options.thumbsOnMarginTop/(options.origWidth/(options.width+2*options.borderWidth))),10)+'px');
                    //theThumb.css('background-position','center '+parseInt((options.thumbsOnMarginTop/(options.origWidth/options.width)),10)+'px');
                    theThumb.css('background-position', 'center ' + parseInt(((options.thumbsOnMarginTop - thumbIconCorrection) / (options.origWidth / options.width)), 10) + 'px');
                    //theThumb.css('background-position','center '+imageInside.css('margin-top').substring(0, imageInside.css('margin-top').length-2)-8+'px');
                    if (thumb_i <= 0) {
                        theThumb.css('margin-left', Math.floor((vp2_html5_bottomPlaylist_thumbsHolderWrapper.width() - vp2_html5_bottomPlaylist_carouselLeftNav.width() - vp2_html5_bottomPlaylist_carouselRightNav.width() - (current_obj.thumbMarginLeft + theThumb.width()) * (options.numberOfThumbsPerScreen - 1) - theThumb.width()) / 2) + 'px');

                    } else {
                        theThumb.css('margin-left', current_obj.thumbMarginLeft + 'px');
                        //alert (current_obj.thumbMarginLeft);
                    }
                });

                current_obj.carouselStep = (thumbsHolder_Thumb.width() + current_obj.thumbMarginLeft) * options.numberOfThumbsPerScreen;/**/

            }

            rearangethumbs();






            var doResize = function () {
                if (!videoIsFullScreen) {
                    responsiveWidth = vp2_html5_bottomPlaylist_border.parent().width();
                    responsiveHeight = vp2_html5_bottomPlaylist_border.parent().height();

                    if (options.responsiveRelativeToBrowser) {
                        responsiveWidth = $(window).width();
                        responsiveHeight = $(window).height();
                    }
                    if (options.width100Proc) {
                        options.width = responsiveWidth - 1;
                    }

                    if (options.height100Proc) {
                        options.height = responsiveHeight;
                    }
                    //alert (options.origWidth+' !=  '+responsiveWidth+' !=  '+options.setOrigWidthHeight);
                    if (options.origWidth != responsiveWidth || options.width100Proc) {
                        if (options.origWidth > responsiveWidth || options.width100Proc) {
                            options.width = responsiveWidth;
                        } else if (!options.width100Proc) {
                            options.width = options.origWidth;
                        }
                        if (!options.height100Proc)
                            options.height = options.width * options.origHeight / options.origWidth;

                        //alert ($(window).width()+' > '+options.width);
                        if ($(window).width() > options.width) {
                            options.width = parseInt(options.width, 10);
                        } else {
                            options.width = parseInt(options.width, 10) - 2 * options.borderWidth;
                        }
                        options.height = parseInt(options.height, 10);
                        var new_height = options.height;
                        if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') == 'none') {
                            new_height = options.height + options.borderWidth + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height();
                        }

                        vp2_html5_bottomPlaylist_Video.css('width', options.width + "px");
                        vp2_html5_bottomPlaylist_Video.css('height', new_height + "px");
                        //container
                        vp2_html5_bottomPlaylist_container.css('width', options.width + "px");
                        vp2_html5_bottomPlaylist_container.css('height', new_height + "px");
                        //controller
                        vp2_html5_bottomPlaylist_controls.css('bottom', videoControllersOrigBottom);
                        vp2_html5_bottomPlaylist_Video_seek.css('width', options.width - options.seekBarAdjust + 'px');
                        vp2_html5_bottomPlaylist_Video_buffer.css('width', vp2_html5_bottomPlaylist_Video_seek.css('width'));
                        //info box
                        vp2_html5_bottomPlaylist_info_box.css('width', options.width - infoBoxAdjust + 'px');


                        if (!options.setOrigWidthHeight) {
                            options.origWidth = options.width;
                            options.origHeight = options.height;
                            videoContainerOrigWidth = vp2_html5_bottomPlaylist_container[0].offsetWidth;
                            videoContainerOrigHeight = vp2_html5_bottomPlaylist_container[0].offsetHeight;
                            //options.setOrigWidthHeight=true;
                        }

                        rearangethumbs();
                        carouselScroll();

                        //border
                        vp2_html5_bottomPlaylist_border.width(vp2_html5_bottomPlaylist_Video.width() + 2 * options.borderWidth);
                        if (vp2_html5_bottomPlaylist_thumbsHolderWrapper.css('display') != 'none') {
                            vp2_html5_bottomPlaylist_border.height(vp2_html5_bottomPlaylist_Video.height() + 3 * options.borderWidth + vp2_html5_bottomPlaylist_thumbsHolderWrapper.height());
                        } else {
                            vp2_html5_bottomPlaylist_border.height(new_height + 2 * options.borderWidth);
                        }

                    }
                } else {
                    if (val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1) {
                        vp2_html5_bottomPlaylist_fullscreen_btn.click();
                    }
                }

            };

            var TO = false;
            $(window).resize(function () {
                doResizeNow = true;

                if (ver_ie != -1 && ver_ie == 9 && current_obj.windowWidth == 0)
                    doResizeNow = false;


                if (current_obj.windowWidth == $(window).width()) {
                    doResizeNow = false;
                    if (options.windowCurOrientation != window.orientation && navigator.userAgent.indexOf('Android') != -1) {
                        options.windowCurOrientation = window.orientation;
                        doResizeNow = true;
                    }
                } else {
                    /*if (current_obj.windowWidth===0 && (val.indexOf("ipad") != -1 || val.indexOf("iphone") != -1 || val.indexOf("ipod") != -1 || val.indexOf("webos") != -1))
						doResizeNow=false;*/
                    current_obj.windowWidth = $(window).width();
                }

                if (options.responsive && doResizeNow) {
                    if (TO !== false)
                        clearTimeout(TO);


                    TO = setTimeout(function () { doResize() }, 300); //300 is time in miliseconds
                }
            });



            if (options.responsive) {
                doResize();
            }




        });
    };

    //
    // plugin skins
    //
    $.fn.vp2_html5_bottomPlaylist_Video.skins = {
    };

})(jQuery);