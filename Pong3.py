import pygame
import math

pygame.init()
gameDisplay = pygame.display.set_mode((800, 600))
pygame.display.set_caption('PONG')
pygame.display.update()

gameExit = False
player1Y = 300
player2Y = 300

white = (255, 255, 255)
black = (0, 0, 0)
red = (255, 0, 0)

ballX = 400
ballY = 300
ballstate = 0

player1Score = 0
player2Score = 0

hit = False
waitFactor = 0

myfont = pygame.font.SysFont("monospace", 15)

    # render text


while not gameExit:
    print(5 - ballX, (player1Y + 50) - ballY)
    if waitFactor < 10:
        print(waitFactor)
        waitFactor += .2
    #print(ballX)
    #(math.hypot(5 - ballX, player1Y - ballY))
    if ballstate == 0:
        ballX += .1
        # Right
    if ballstate == 1:
        ballX -= .1
        # Left
    if ballstate == 2:
        ballX += .1
        ballY += .1
        # BottomRight
    if ballstate == 3:
        ballX += .1
        ballY -= .1
        # TopRight
    if ballstate == 4:
        ballX -= .1
        ballY -= .1
        # TopLeft
    if ballstate == 5:
        ballX -= .1
        ballY += .1
        # BottomLeft

    if ballY <= 0:
        if ballstate == 3:
            ballstate = 2
        if ballstate == 4:
            ballstate = 5
    if ballY >= 600:
        if ballstate == 2:
            ballstate = 3
        if ballstate == 5:
            ballstate = 4

    if ballX <= 20:

        if math.hypot(5 - ballX, (player1Y + 50) - ballY) <= 50:
            print("hit paddle")
            if ballstate == 1:
                if math.hypot(5 - ballX, (player1Y + 50) - ballY) >= 25:
                    if waitFactor >= 10:
                        ballstate = 4
                        waitFactor = 0
                else:
                    ballstate = 0
            if ballstate == 4:
                if waitFactor >= 10:
                    ballstate = 3
                    waitFactor = 0
            if ballstate == 5:
                if waitFactor >= 10:
                    ballstate = 2
                    waitFactor = 0
            if ballstate == 2:
                if waitFactor >= 10:
                    ballstate = 5
                    waitFactor = 0
            if ballstate == 3:
                if waitFactor >= 10:
                    ballstate = 4
                    waitFactor = 0
        else:
            if ballX <= 0:
                player2Score += 1
                ballX = 400
                ballY = 300
                ballstate = 1
    if ballX >= 785:

        if math.hypot(785 - ballX, (player2Y + 50) - ballY) <= 50:
            print("hit paddle")
            if ballstate == 0:
                if math.hypot(785 - ballX, (player2Y + 50) - ballY) >= 25:
                    if waitFactor >= 10:
                        ballstate = 4
                        waitFactor = 0
                else:
                    ballstate = 1
            if ballstate == 4:
                if waitFactor >= 10:
                    ballstate = 3
                    waitFactor = 0
            if ballstate == 5:
                if waitFactor >= 10:
                    ballstate = 2
                    waitFactor = 0
            if ballstate == 2:
                if waitFactor >= 10:
                    ballstate = 5
                    waitFactor = 0
            if ballstate == 3:
                if waitFactor >= 10:
                    ballstate = 4
                    waitFactor = 0
        else:
            if ballX >= 800:
                player1Score += 1
                ballX = 400
                ballY = 300
                ballstate = 1

    for event in pygame.event.get():

        if event.type == pygame.KEYUP:
            if event.key == pygame.K_w:
                print("w")
                if player1Y > 0:
                    player1Y -= 20
            if event.key == pygame.K_s:
                print("s")
                if player1Y < 500:
                    player1Y += 20
            if event.key == pygame.K_UP:
                if player2Y > 0:
                    player2Y -= 20
            if event.key == pygame.K_DOWN:
                if player2Y < 500:
                    player2Y += 20
        if event.type == pygame.QUIT:
            gameExit = True
    gameDisplay.fill(black)
    pygame.draw.rect(gameDisplay, white, [5, player1Y, 10, 100])

    pygame.draw.rect(gameDisplay, white, [785, player2Y, 10, 100])
    pygame.draw.rect(gameDisplay, white, [ballX, ballY, 10, 10])

    ScoreA = myfont.render(str(player1Score), 1, (255, 255, 255))
    ScoreB = myfont.render(str(player2Score), 1, (255, 255, 255))
    gameDisplay.blit(ScoreA, (300, 100))
    gameDisplay.blit(ScoreB, (500, 100))
    pygame.display.update()

pygame.quit()
quit()