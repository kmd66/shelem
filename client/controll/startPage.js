import { Propertys as p } from './public';

export class StartPage {
    constructor() {
    }

    init() {
        const html = `<div class="flexC around">
            ${img()}
            <div class="flex around">
            ${userInfo(p.user.Info, p.user.Level)}
            ${userInfo(p.rivaUser.Info, p.rivaUser.Level)}
            </div>
            <div class="textCenter">
                <button class="btn btn-Greenlight">شروع</button>
            </div>
        </div>
        <div class="startPageProgressBar">
            <span>انصراف</span>
            <div></div>
        </div>`;

        const _startPage = document.getElementById('startPage');
        _startPage.innerHTML = html;
        _startPage.style.display = 'block'; 

        let startButton;
        const interval = setInterval(() => {
            startButton = document.querySelector('#startPage .btn-Greenlight');
            if (startButton) {
                this.progressBar = document.querySelector('.startPageProgressBar div');
                events();
                this.lastTime = Date.now();
                this.updateProgress();
                clearInterval(interval);
            }
        }, 50);

        function img(img1, img2) {
            return `<div class="flex around">
                <div class="imgC">
                    <img src="${p.user.Info.Img}90.jpg" />
                </div>
                <div class="imgC">
                    <img src="${p.rivaUser.Info.Img}90.jpg" />
                </div>
            </div>`;
        }
        function userInfo(info, level) {
            return `
                <div class="startPageText">
                    <div>${info.FirstName} ${info.LastName}</div>
                    <div>${info.UserName}</div>
                    <div>سطح: ${level}</div>
                </div>
            `;
        }
        const startPageHide = this.hide;
        function events() {
            startButton.onclick = () => startPageHide();
            //startButton.onclick = () => socket.connection.invoke("Start", p.keys.roomId, p.keys.key);
        }
    }

    hide() {
        document.getElementById('startPage').innerHTML = '';
        document.getElementById('startPage').style.display = 'none';
    }

    updateProgress() {
        if (p.start || p.delay.Wait < 0) {
            progressBar.style.width = '0px';
            return;
        }

        const now = Date.now();
        const delta = (now - this.lastTime) / 1000;
        this.lastTime = now;

        p.delay.Wait -= delta;
        const percent = (p.delay.Wait / p.delay.Total) * 100;
        this.progressBar.style.width = percent + '%';
        requestAnimationFrame(() => this.updateProgress());
    }

    ReceiveStart() {
        let startButton = document.querySelector('#startPage .textCenter');
        if (startButton) {
            startButton.innerHTML = 'درانتضار تایید کاربر';
        }
    }

    ReceiveCansel() {
        const html = `<div class="flexC around">
            <div style="text-align: center;">
            <div class="msgCancel">شروع بازی تایید نشد </div>
            <button class="btn btnCancel">بازگشت</button>
        </div>`;
        document.getElementById('startPage').innerHTML = html;
        const windowClose = this.windowClose;
        const interval = setInterval(() => {
            let btnCancel = document.querySelector('#startPage .btnCancel');
            if (btnCancel) {
                clearInterval(interval);
                btnCancel.onclick = () => windowClose();
            }
        }, 50);
    }

    windowClose() {
        if (window.flutter_inappwebview)
            window.flutter_inappwebview.callHandler('f_urlBack');
        else
            window.close();
    }
}

