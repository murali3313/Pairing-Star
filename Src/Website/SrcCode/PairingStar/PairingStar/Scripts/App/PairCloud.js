﻿var pairingStar = {};

(function () {

    // notice there are no dragOptions specified here, which is different from the
    // draggableConnectors2 demo.  all connections on this page are therefore
    // implicitly in the default scope.
    var endpoint = {
        anchor: [0.5, 0.5, 0, -1],
        connectorStyle: { lineWidth: 7, strokeStyle: "rgba(198,89,30,0.7)" },
        endpointsOnTop: true,
        isSource: true,
        maxConnections: 10,
        isTarget: true,
        dropOptions: { tolerance: "touch", hoverClass: "dropHover" }
    },
        
        discs = [];


    pairingStar.pairPlumb = {
        init: function () {

            jsPlumb.importDefaults({
                DragOptions: { cursor: 'wait', zIndex: 20 },
                Endpoint: ["Image", { url: "../img/littledot.png" }],
                Connector: ["Bezier", { curviness: 90 }]
            });

            var e1 = prepare("bd1"),
                e2 = prepare("bd2"),
                e3 = prepare("bd3"),
                e4 = prepare("bd4");

            pairPlumb.initClearButton();

            jsPlumb.connect({ source: e1, target: e2 });
            jsPlumb.connect({ source: e1, target: e3 });
            jsPlumb.connect({ source: e1, target: e4 });

            pairPlumb.initAddButton();
        },
        prepare : function (elId) {
            this.initHover(elId);
            this.initAnimation(elId);

            return jsPlumb.addEndpoint(elId, endpoint);
        },

        // this is overridden by the YUI demo.
        createDisc: function () {
            var d = document.createElement("div");
            d.className = "bigdot";
            document.getElementById("pairingCloud").appendChild(d);
            var id = '' + ((new Date().getTime()));
            var _d = jsPlumb.CurrentLibrary.getElementObject(d);
            jsPlumb.CurrentLibrary.setAttribute(_d, "id", id);
            var w = screen.width - 162, h = screen.height - 162;
            var x = (0.2 * w) + Math.floor(Math.random() * (0.5 * w));
            var y = (0.2 * h) + Math.floor(Math.random() * (0.6 * h));
            d.style.top = y + 'px';
            d.style.left = x + 'px';
            return { d: d, id: id };
        },

        addDisc: function () {
            var info = this.createDisc();
            var e = this.prepare(info.id);
            jsPlumb.draggable(info.id);
            discs.push(info.id);
        },

        reset: function () {
            for (var i = 0; i < discs.length; i++) {
                var d = document.getElementById(discs[i]);
                if (d) d.parentNode.removeChild(d);
            }
            discs = [];
        },
        initClearButton: function () {
            $("#clear").unbind("click", jsPlumb.detachEveryConnection);
            $("#clear").bind("click", jsPlumb.detachEveryConnection);
        },

        initAddButton: function () {
            $("#add").unbind("click", addDisc);
            $("#add").bind('click', addDisc);
        },

        initHover: function (elId) {
            $("#" + elId).hover(
                function () { $(this).addClass("bigdot-hover"); },
                function () { $(this).removeClass("bigdot-hover"); }
            );
        },

        initAnimation: function (elId) {
            $("#" + elId).bind('click', function (e, ui) {
                if ($(this).hasClass("jsPlumb_dragged")) {
                    $(this).removeClass("jsPlumb_dragged");
                    return;
                }
                var o = $(this).offset();
                var w = $(this).outerWidth();
                var h = $(this).outerHeight();
                var c = [o.left + (w / 2) - e.pageX, o.top + (h / 2) - e.pageY];
                var oo = [c[0] / w, c[1] / h];
                var l = oo[0] < 0 ? '+=' : '-=', t = oo[1] < 0 ? "+=" : '-=';
                var DIST = 450;
                l = l + Math.abs(oo[0] * DIST);

                t = t + Math.abs(oo[1] * DIST);
                // notice the easing here.  you can pass any args into this animate call; they
                // are passed through to jquery as-is by jsPlumb.
                var id = $(this).attr("id");
                jsPlumb.animate(id, { left: l, top: t }, { duration: 1400, easing: 'easeOutBack' });
            });
        }
    };

})();



