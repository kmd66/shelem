import { gsap } from "gsap";
import { Propertys as p } from './public';

class StartPage {
    constructor() {
    }

    static instance;
    static Instance() {
        if (!StartPage.instance) {
            StartPage.instance = new StartPage();
        }
        return StartPage.instance;
    }

    init() {
        p.start = false;
        p.delay = {
            Wait: 60,
            Total: 60
        }
        const model = {
            img1:'/test/p1',
            img2: '/test/p2',

            name1: 'name1',
            fName1: 'fName1',
            name2: 'name2',
            fName2: 'fName2',

            uName1: 'fName1',
            uName2: 'fName2',

            level1: '1',
            level2: '1'
        }

        const html = `<div class="flexC around">
            ${img(model.img1, model.img2)}
            <div class="flex around">
            ${userInfo(`${model.name1} ${model.fName1}`, model.uName1, model.level1)}
            ${userInfo(`${model.name2} ${model.fName2}`, model.uName2, model.level2)}
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
                    <img src="${img1}90.jpg" />
                </div>
                <div class="imgC">
                    <img src="${img2}90.jpg" />
                </div>
            </div>`;
        }
        function userInfo(name, uName, level) {
            return `
                <div class="startPageText">
                    <div>${name}</div>
                    <div>${uName}</div>
                    <div>سطح: ${level}</div>
                </div>
            `;
        }
        function events() {
            startButton.onclick = () => startPage.hide();
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

if (!window.__startPage__) {
    window.__startPage__ = StartPage.Instance();
}
export const startPage = window.__startPage__;
