import { Propertys as p } from './public';
import { gsap } from "gsap";

export class ElHokm {
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

    addSelectPoint(minSelect, time, my) {
        const div = document.getElementById('selectPoint');
        if (minSelect == undefined) {
            div.innerHTML = '';
            div.style.display = 'none';
            return;
        }
        const points = [100, 105, 110, 115, 120, 125, 130, 135, 140, 150, 155, 160];

        let h = '<div class="selectPoint"><div class="selectPointBtn btn s" data-p="365">شلم</div>';
        for (const point of points) {
            let d = point > minSelect ? '' : 'd';
            h += `<div class="selectPointBtn btn ${d}" data-p="${point}">${point}</div>`;
        }
        h += '<div class="selectPointBtn btn p" data-p="0">پاس دادن</div>';
        h += `<div class="selectPointBar"><div></div></div></div>`;
        div.innerHTML = h;
        div.style.display = 'block';
    }

    async containerMainRemove(my) {
        const cards = document.querySelectorAll('#containerMain .card');
        const tl = gsap.timeline();
        tl.to(cards, {
            x: 0,
            y: 0,
            rotation: 0,
            duration: 0.5,
            ease: "power2.out"
        })
            .to(cards, {
                top: my ? p.h : 0,
                duration: 0.5, //stagger: 0.1,
                ease: "power2.out"
            });

        await tl.then();
        document.getElementById('containerMain').innerHTML = '';
    }
}
