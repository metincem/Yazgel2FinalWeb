var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Critical).withUrl("/GHub").build();
var cntr = 240;
var scsAudio = document.getElementById("successAudio"); 
var wrgAudio = document.getElementById("wrongAudio"); 
$(function () {
    function rotateDiv(pt, card) {
        $("#c-" + card).children(".frame").children("i").hide("fast");
        $("#c-" + card).animate({ trans: 1080 }, {
            step: function (now, fx) {

                $("#c-" + card).css('transform', 'rotateY(' + now + 'deg)');
            },
            duration: 'slow',
            complete: function () {
                $("#c-" + card).children(".frame").css("background-image", "url(" + pt + ")");
                //$("#c-" + card).children(".frame").children("i").hide("fast");
            }
        }, 'linear');
    }

    function rotateDivO(card) {
        $("#c-" + card).animate({ trans: 0 }, {
            step: function (now, fx) {

                $("#c-" + card).css('transform', 'rotateY(' + now + 'deg)');
            },
            duration: 'slow',
            complete: function () {
                $("#c-" + card).children(".frame").css("background-image", "none");
                $("#c-" + card).children(".frame").children("i").removeClass();
                $("#c-" + card).children(".frame").children("i").addClass("icofont-question");
                $("#c-" + card).children(".frame").children("i").show("fast");
            }
        }, 'linear');
    }

    connection.start().then(function () {
        console.log("Bağlantı Oluşturuldu.");
        connection.invoke("GameStarted");
        $(".time-box").text("4:00");
        setInterval(function () {
            if (cntr > 0) {
                var dk = Math.floor(cntr / 60);
                var sn = cntr % 60;

                if (sn < 10) {
                    sn = "0" + sn.toString();
                }
                cntr--;
                $(".time-box").text(dk + ":" + sn);
            }            
        }, 1000);
    }).catch((err) => console.log(err))
    connection.on("Status", (upt) => {
        window.location.href = upt;
    });
    connection.on("receiveCards", (cards) => {
        $(".cards").html("");
        for (var i = 0; i < cards.length; i++) {
            var cardDiv = $("<div></div>").addClass("card").attr("id", "c-" + i);
            var frameDiv = $("<div></div>").addClass("frame").addClass("center");
            var iTag = $("<i></i>").addClass("icofont-question");
            var iSpan = $("<span></span>").text(cards[i]).css("display", "none");
            $(frameDiv).append(iTag);
            $(cardDiv).append(frameDiv);
            $(cardDiv).append(iSpan);
            $(".cards").append(cardDiv);
        }
    });
    connection.on("receiveSv", (pt, card) => {
        rotateDiv(pt, card);
    });

    connection.on("cmpltd", (fr, sc, status, pt) => {
        if (status == 1) {
            rotateDiv(pt, sc);
            scsAudio.play();
            $("#c-" + fr).css("color", "green");
            $("#c-" + sc).css("color", "green");
            $("#c-" + fr).children(".frame").children("i").removeClass();
            $("#c-" + fr).children(".frame").children("i").addClass("icofont-verification-check");
            $("#c-" + fr).css("background-color", "green");
            $("#c-" + sc).children(".frame").children("i").removeClass();
            $("#c-" + sc).children(".frame").children("i").addClass("icofont-verification-check");
            $("#c-" + sc).css("background-color", "green");
            $("#c-" + fr).children(".frame").children("i").show("fast");
            $("#c-" + sc).children(".frame").children("i").show("fast");
            
            setTimeout(function () {
                $("#c-" + sc).css("opacity", "0");
                $("#c-" + fr).css("opacity", "0");
            }, 1500);
            setTimeout(function () {
                $("#c-" + sc).css("visibility", "hidden");
                $("#c-" + fr).css("visibility", "hidden");
            }, 1700);
            
        }
        else {
            rotateDiv(pt, sc);
            wrgAudio.play();
            $("#c-" + fr).children(".frame").children("i").removeClass();
            $("#c-" + fr).children(".frame").children("i").addClass("icofont-close");
            $("#c-" + fr).css("background-color", "red");
            $("#c-" + sc).children(".frame").children("i").removeClass();
            $("#c-" + sc).children(".frame").children("i").addClass("icofont-close");
            $("#c-" + sc).css("background-color", "red");
            $("#c-" + fr).children(".frame").children("i").show("fast");
            $("#c-" + sc).children(".frame").children("i").show("fast");
            setTimeout(function () {
                $("#c-" + fr).children(".frame").children("i").removeClass();
                $("#c-" + fr).children(".frame").children("i").addClass("icofont-close");
                $("#c-" + fr).css("background-color", "#4834d4");
                $("#c-" + sc).children(".frame").children("i").removeClass();
                $("#c-" + sc).children(".frame").children("i").addClass("icofont-close");
                $("#c-" + sc).css("background-color", "#4834d4");
                rotateDivO(sc);
                rotateDivO(fr);
            }, 1500);
        }
    });

    $("body").on("click", ".card", function () {
        if ($(this).css("transform") != "matrix3d(1, 0, 7.34788e-16, 0, 0, 1, 0, 0, -7.34788e-16, 0, 1, 0, 0, 0, 0, 1)") {
            var gd = $(this).children("span").text();
            connection.invoke("SendGuid", gd);
        }
    });
});