import { Propertys as p } from './public';
export class ElsTop {
    addBar() {
        const _userTemp = this.userTemp();
        const _chatTemp = this.chatTemp();
        const _infoTemp = this.infoTemp();
        const _heder = document.getElementById('heder');
        const _foter = document.getElementById('foter');

        _heder.appendChild(_userTemp.rivaUser);
        _heder.appendChild(_chatTemp);
        _foter.appendChild(_userTemp.user);
        _foter.appendChild(_infoTemp);

        const interval = setInterval(() => {
            let sdf56s1 = heder.querySelector('.chatBox');
            if (sdf56s1) {
                clearInterval(interval);
                this.sdf56s1 = sdf56s1;
                //this.updateProgress();
            }
        }, 50);
    }

    formatTime(seconds) {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
    }

    async updateProgress() {
        await new Promise(r => setTimeout(r, 1000));
        p.delay.Wait = p.delay.Wait - 1;
        this.sdf56s1.innerHTML = this.formatTime(p.delay.Wait);
        this.updateProgress();
    }

    chatTemp() {
        const div = document.createElement('div');
        div.className = 'chatBox flex';
        div.innerHTML = `
            <div class="iconBtn"><i class="icon-messages-2"></i></div>
            <div class="iconBtn"><i class="icon-message-text"></i></div>
            <div class="iconBtn"><i class="icon-smileys"></i></div>`;
        return div;
    }

    infoTemp() {
        const div = document.createElement('div');
        div.className = 'chatBox flex';
        div.innerHTML = `
            <div class="iconBtn"><i class="icon-information4"></i></div>
            <div class="iconBtn"><i class="icon-card-edit"></i></div>`;
        return div;
    }

    userTemp() {
        return {
            user: info(p.user.Info, p.user.Level, 'user'),
            rivaUser: info(p.rivaUser.Info, p.rivaUser.Level, 'rivaUser'),
        }
        function info(info, level, className) {
            const div = document.createElement('div');
            div.className = 'userInfo flex';
            div.innerHTML = `
                <div class="sdf56s2 ${className}"><img src="${info.Img}90.jpg" /></div>
                <div class="sdf56s4">
                    <div>${info.UserName}</div>
                    <div><i class="icon-star4"></i><span>${level}</span></div>
                </div>`;
            return div;
        }
    }

    receiveMainTime(model) {
        let user = document.querySelector('.sdf56s2.user');
        let rivaUser = document.querySelector('.sdf56s2.rivaUser');

        this.receiveMainTime2(user, p.user.Id == model.tourner ? model : undefined);
        this.receiveMainTime2(rivaUser, p.user.Id == model.tourner ? undefined : model);
    }

    receiveMainTime2(el, model) {
        if (!model || model.i == 0) {
            el.style.background = 'transparent';
        }
        else {
            try {
                const darsad = (model.i * 100) / model.shotTime;
                const deg = (darsad / 100) * 360;
                el.style.background = `conic-gradient(from 0deg, #03dac6 0deg ${deg}deg, black ${deg}deg 360deg) border-box`;
            } catch { }
        }
    }


    addHokm(hokm, my) {
        const hokmDiv = document.getElementById('hokm');
        if (hokm == undefined) {
            hokmDiv.innerHTML = '';
            hokmDiv.style.display = 'none';
            return;
        }

        let h;
        switch (hokm) {
            case 0: h = '♠'; break;
            case 1: h = '♥'; break;
            case 2: h = '♣'; break;
            case 3: h = '♦'; break;
        }
        if (my) {
            hokmDiv.style.top = 'calc(100vh - 5.9rem)';
            hokmDiv.style.borderRadius = '50% 50% 0 0';
        }
        else {
            hokmDiv.style.top = '3.9rem';
            hokmDiv.style.borderRadius = '0 0 50% 50%';
        }

        hokmDiv.innerHTML = h;
        hokmDiv.style.display = 'block';
    }
}
