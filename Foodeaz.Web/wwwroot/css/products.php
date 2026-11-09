<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Americano</title>
    <link rel="icon" type="image/png" sizes="32x32" href="img/fav.png" />
    <?php include 'includes/styles.php'?>
    <?php include 'includes/data.php'?>
</head>

<body>

<?php include 'includes/header.php'?>
    <!-- main -->
    <main class="subpage">
        <?php include 'includes/page-contact.php'?>
        
        <!-- sub page header -->
        <section class="subpage-header">
            <div class="container">
                 <div class="row">
                    <div class="col-md-6">
                        <h5 class="h6 text-uppercase position-relative">Flavours</h5>
                        <h1 class="h1 fbold">Our Products</h1>                       
                    </div>
                </div>
            </div>
        </section>
        <!--/ sub page heder -->

         <!-- sub page body -->
         <section class="subpage-body">
            <div class="custom-container home-services">
                <div class="row">                  
                    <?php 
                        for($i=0; $i<count($serviceItem); $i++) { ?>
                        <div class="col-lg-4">
                           <div class="swiper-slide mb-4">
                                <figure>
                                    <a href="<?php echo $serviceItem [$i][3]?>"><img src="img/products/<?php echo $serviceItem [$i][0]?>.png" alt=""></a>
                                </figure>
                                <article class="p-5">
                                    <h3><?php echo $serviceItem [$i][1]?></h3>
                                    <p class="pb-1"><?php echo $serviceItem [$i][2]?></p>
                                    <!-- <a href="<?php echo $serviceItem [$i][3]?>" class="h6"><span class="icon-arrow-right icomoon"></span> Read More</a> -->
                                </article>
                           </div>
                        </div>  
                    <?php } ?>  
                </div>
            </div>
         </section>
        <!--/ sub page body -->

    </main>
    <!--/ main -->

    <!-- footer -->
    <?php include 'includes/footer.php'?>
    <!--/ footer -->
    <?php include 'includes/scripts.php'?>

</body>

</html>