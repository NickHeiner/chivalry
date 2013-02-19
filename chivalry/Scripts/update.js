/**
 * These hooks are deployed via WAMS,
 * but I want to version them alongside the rest of the code.
 */

function update(item, user, request) {

    function notify(toNotify) {
        push.wns.sendRaw(
            toNotify,
            item + ', ' + user + ', ' + request,
            {
                client_id: 'ms-app://s-1-15-2-1845371367-2612145538-294013343-2430892689-996354481-418617520-201036955',
                client_secret: '/u4BRjIFgU3xZN2DoTyB0EPT7fJhHqE5'
            }, function (err, result) {
                if (err) console.error(err);
                if (result) console.log(result);
            }
        );
    }

    request.execute({
        success: function () {
            request.respond();
            notify(item.RecepientChannelId);
            notify(item.InitiatorChannelId);
        }
    });

}