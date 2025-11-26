import { Deck } from './deck';
import { gsap } from "gsap";
import { DeckControllExtention } from "./deckControllExtention";
import { Propertys as p } from './public';
class DeckControll {
    constructor() {
        this.container = document.getElementById('container');
        this.extention = new DeckControllExtention();
        this.addevent()
    }

    static instance;
    static Instance() {
        if (!DeckControll.instance) {
            DeckControll.instance = new DeckControll();
        }
        return DeckControll.instance;
    }

    addevent() {

        const container = document.getElementById('container');

        container.addEventListener('mousemove', (e) => this.extention.events.onDrag(e));
        container.addEventListener('touchmove', (e) => this.extention.events.onDrag(e));
        container.addEventListener('mouseup', (e) => this.extention.events.endDrag(e));
        container.addEventListener('touchend', (e) => this.extention.events.endDrag(e));
    }

    async start() {
        this.extention.removeAllDesk(p.startDeck);
        this.extention.removeAllDesk(p.myFan, true);
        this.extention.removeAllDesk(p.rivaFan, true);
        this.extention.removeAllDesk(p.myGround, true);
        this.extention.removeAllDesk(p.rivaGround, true);

        const container = document.getElementById('container');
        container.innerHTML = '';
        p.startDeck = Deck();
        await shuffle(p.startDeck);

        const tl = gsap.timeline();
        const tl2 = gsap.timeline();

        const cardsToSpread1 = p.startDeck.cards.slice(0, 24);
        const cardsToSpread2 = p.startDeck.cards.slice(24, 48);

        cardsToSpread1.forEach((card, index) => {
            const y = index % 2 === 0 ? p.hC : p.hC*-1;
            tl.to(card.$el, {
                x: 0 + index* -0.2,
                y: y,
                duration: 0.5,
            }, index * 0.05);
        });
        await tl.then();

        const yN = p.hC - p.remToPx(7);
        const xN = p.remToPx(7);
        cardsToSpread2.forEach((card, index) => {
            const y = index % 2 === 0 ? yN : yN * -1;
            let x = xN;
            if (index > 15) x = xN * -1;
            else if (index > 7) x = 0;
            tl2.to(card.$el, {
                x: x + index * -0.2,
                y: y,
                duration: 0.5,
            }, index * 0.05);
        });
        await tl2.then();

        this.extention.removeAllCards(p.startDeck, cardsToSpread1);
        this.extention.removeAllCards(p.startDeck, cardsToSpread2);
        async function shuffle(d) {
            d.mount(container);
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
            d.shuffle();
            await new Promise(resolve => setTimeout(resolve, 700));
        }
    }

    async startFan(list) {
        this.extention.removeAllDesk(p.myFan, true);
        this.extention.removeAllDesk(p.rivaFan, true);

        const container = document.getElementById('container');

        p.myFan = this.extention.createFanCarts(list, true);
        p.rivaFan = this.extention.createFanCarts(list, false);

        //this.myFan.forEach((c) => document.getElementById('containerMyFan').appendChild(c.el));
        //this.rivaFan.forEach((c) => document.getElementById('containerRivaFan').appendChild(c.el));
        p.myFan.forEach((c) => container.appendChild(c.el));
        p.rivaFan.forEach((c) => container.appendChild(c.el));
    }

    async startGround(list) {
        this.extention.removeAllDesk(this.myGround, true);
        this.extention.removeAllDesk(this.rivaGround, true);

        const container = document.getElementById('container');

        p.myGround = this.extention.createGroundCarts(list.u1, true);
        p.rivaGround = this.extention.createGroundCarts(list.u2, false);

        p.myGround.forEach((c) => container.appendChild(c.el));
        p.rivaGround.forEach((c) => container.appendChild(c.el));

    }
    
}

if (!window.__deckControll__) {
    window.__deckControll__ = DeckControll.Instance();
}
export const deckControll = window.__deckControll__;
