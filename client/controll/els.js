import { Propertys as p } from './public';
import { ElsTop } from './elsTop';
import { StartPage } from './startPage';

class Els {
    constructor() {
        this.top = new ElsTop();
        this.startPage = new StartPage();
    }

    static instance;
    static Instance() {
        if (!Els.instance) {
            Els.instance = new Els();
        }
        return Els.instance;
    }

}

if (!window.__els__) {
    window.__els__ = Els.Instance();
}
export const els = window.__els__;