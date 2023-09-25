# judge

### Setup

Loyiha Ubuntu 18.04 ichida ishlash uchun mo'ljallangan. Sababi Compilerlar shu sistemada sinab ko'rilgan hozircha. Keyinchalik so'ngi versiyalariga upgrade qilinadi.

##### Run
Loyihani ishga tushirish uchun `docker-compose up -d` komandasi yetarli. Shundan keyin **insomnia** papkasidagi yaml faylni Insomnia app ga import qilib,  dasturni sinab ko'rsangiz bo'ladi.
> Insomnia collection ichida 2 xil environment bor: 1. Base, 2. Docker. Docker orqali ishga tushirilganda Insomnia env. ham docker ga o'zgartirish kerak.


##### Debug
Kodga yangilik qo'shgandan keyin uni debug qilish uchun Windows ishlatuvchilar WSL ishlatsa bo'ladi.
1. VS Code uchun WSL extension o'rgnatib oling.
2. CTRL+SHIFT+P ni bosib WSL extensionni qidiring
3. Undan keyin WSLni versiyasini tanlab yangi oynada oching. 
4. Ochilgandidan keyin "Open Folder" tugmasini bosib loyihani shu WSL ichida vscode ga yuklang.
    a. Menda `/mnt/c/dev/judge` papkasida joylashgan. Siz o'zingiz saqlab olgan papkani tanlang.
