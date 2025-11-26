
import { Propertys as p } from './public';
export class DeckEvents {
    constructor() {
    }

    init(div, my) {
        if (!my) return;

        div.addEventListener('mousedown', (e) => this.startDrag(e));
        div.addEventListener('touchstart', (e) => this.startDrag(e));
    }

    getRelativePos(e) {
        let x, y;

        if (e.touches && e.touches.length > 0) {
            // موبایل: لمس فعال
            x = e.touches[0].clientX;
            y = e.touches[0].clientY;
        } else if (e.changedTouches && e.changedTouches.length > 0) {
            // موبایل: پایان لمس
            x = e.changedTouches[0].clientX ;
            y = e.changedTouches[0].clientY;
        } else {
            // دسکتاپ
            x = e.clientX ;
            y = e.clientY ;
        }

        return { x, y };
    }

    startDrag(e) {
        this.dragId = e.currentTarget.id;
        this.card = p.getCardById(this.dragId);
        if (this.card.rank == undefined || this.card.rank < 1) return;
        //this.rect = this.card.el.getBoundingClientRect();
        this.transform = this.card.el.style.transform;
        this.isDragging = true;

        const pos = this.getRelativePos(e);
        this.setLoc(pos);
    }

    setLoc(pos) {
        const posX = pos.x - (this.card.el.offsetWidth / 2);
        const posY = pos.y - (this.card.el.offsetHeight / 2);
        this.card.el.style.transform = '';
        this.card.el.style.left = `${posX}px`;
        this.card.el.style.top = `${posY}px`;

    }

    onDrag(e) {
        if (!this.isDragging) return;
        //e.preventDefault();

        const pos = this.getRelativePos(e);
        this.setLoc(pos);

    }

    endDrag(e) {
        if (!this.isDragging) return;

        this.card.el.style.left = `${this.card.loc.x}px`;
        this.card.el.style.top = `${this.card.loc.y}px`;

        this.card.el.style.transform = this.transform;

        this.isDragging = false;
        this.dragId = null;
    }
}

